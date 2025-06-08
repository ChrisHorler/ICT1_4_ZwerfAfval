using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ControlApi.API.Controllers;
using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ControlApi.Tests;

public class AuthControllerTests
{
    // Helpers
    private static ControlApiDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ControlApiDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        
        return new ControlApiDbContext(options);
    }

    private static (AuthController ctrl, Mock<IJwtService> jwtMock, ControlApiDbContext db) CreateSut()
    {
        var db = CreateInMemoryDb();
        var jwtMock = new Mock<IJwtService>();
        jwtMock.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("fakeToken");
        
        var ctrl = new AuthController(db, jwtMock.Object);
        return (ctrl, jwtMock, db);
    }
    
    // Tests
    [Fact]
    public async Task Register_NewEmail_ReturnsOkWithToken()
    {
        // Arrange
        var (ctrl, _, _) = CreateSut();
        var dto = new RegisterRequest("new@ex.com", "pass");

        // Act
        var result = await ctrl.Register(dto);

        // Assert
        var ok   = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("new@ex.com", body.email);
        Assert.Equal("fakeToken",   body.token);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var (ctrl, _, db) = CreateSut();
        db.users.Add(new User { email = "dupe@ex.com", passwordHash = "x" });
        await db.SaveChangesAsync();

        var dto = new RegisterRequest ("dupe@ex.com","pass");

        // Act
        var result = await ctrl.Register(dto);

        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task Login_CorrectCreds_ReturnsOkWithToken()
    {
        // Arrange
        var (ctrl, _, db) = CreateSut();
        var hash = BCrypt.Net.BCrypt.HashPassword("secret");
        db.users.Add(new User { email = "me@ex.com", passwordHash = hash });
        await db.SaveChangesAsync();
        var dto = new LoginRequest ("me@ex.com","secret");

        // Act
        var result = await ctrl.Login(dto);

        // Assert
        var ok   = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("fakeToken", body.token);
    }
    
    [Fact]
    public async Task Login_WrongPassword_ReturnsConflict()
    {
        // Arrange
        var (ctrl, _, db) = CreateSut();
        var hash = BCrypt.Net.BCrypt.HashPassword("secret");
        db.users.Add(new User { email = "me@ex.com", passwordHash = hash });
        await db.SaveChangesAsync();
        var dto = new LoginRequest ( "me@ex.com", "wrong");

        // Act
        var result = await ctrl.Login(dto);

        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnBadRequest()
    {
        // Arrange
        var (ctrl, _, _) = CreateSut();
        var dto = new RegisterRequest("test@example.com", "weakpass1");
        
        // Manually validate model
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto, null, null);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);

        foreach (var result in validationResults)
        {
            ctrl.ModelState.AddModelError(result.MemberNames.First(), result.ErrorMessage);
        }
        
        // Act
        var response = await ctrl.Register(dto);
        
        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(response.Result);
        var errors = Assert.IsType<SerializableError>(badRequest.Value);

        Assert.True(errors.ContainsKey("password"), "Expected validation error for password.");

    }
}