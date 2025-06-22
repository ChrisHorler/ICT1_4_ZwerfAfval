using Frontend_Dashboard.Components;
using Frontend_Dashboard.Components.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Net;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMemoryCache();
builder.Services.AddBlazorBootstrap();
builder.Services.AddSingleton<DateService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<IAnalyticsDataService, AnalyticsDataService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp
    => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<AuthService>();

// JWT Authentication
var jwtKey = builder.Configuration["JWT_KEY"]
    ?? Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT_KEY environment variable not set");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = false,
            ValidateAudience         = false,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = signingKey
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var apiBase = builder.Configuration["BackendAPI:BaseUrl"]
              ?? throw new InvalidOperationException("API Base URL Not Set");

builder.Services.AddHttpClient("BackendApi", client =>
{
    var baseUrl = builder.Configuration["API_BASE_URL"];
    if (string.IsNullOrEmpty(baseUrl))
        throw new Exception("API_BASE_URL enviromental variable not detected");

    client.BaseAddress = new Uri(baseUrl);
});


builder.Services.AddHttpClient<CalendarService>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<FunFactsService>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
   // app.UseHttpsRedirection(); 
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();