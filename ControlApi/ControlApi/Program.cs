using System.Text;
using ControlApi;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.SensoringConnection.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

const bool TESTING = false;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default");

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
if (string.IsNullOrEmpty(connectionString))
{
    if (string.IsNullOrEmpty(config["CONN_STRING"]))
    {
        throw new InvalidOperationException("No connection string supplied");
    }
    connectionString = config["CONN_STRING"];
}



    
builder.Services.AddHttpClient();

// --- Prediction-API typed client ---
builder.Services.AddHttpClient<IPredictionApiClient, PredictionApiClient>(client =>
{
    client.BaseAddress = new Uri(
        Environment.GetEnvironmentVariable("PREDICTION_API"));
    client.Timeout = TimeSpan.FromSeconds(15);
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- MariaDB ---
builder.Services.AddDbContext<ControlApiDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString))); 

// --- Authorization ---
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? Environment.GetEnvironmentVariable("JWT_KEY");

if (string.IsNullOrEmpty(jwtKey))
{
    if (string.IsNullOrEmpty(config["JWT_KEY"]))
    {
        throw new InvalidOperationException("No JWT_KEY supplied");
    }
    jwtKey = config["JWT_KEY"];
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    ["Testing"] = $"{TESTING}"  // or "false"
});
builder.Services.AddHostedService<DailyBackgroundService>();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ControlApiDbContext>();
//     db.Database.Migrate();
// }

app.Run();

namespace ControlApi
{
    
}