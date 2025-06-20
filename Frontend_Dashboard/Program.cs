using Frontend_Dashboard.Components;
using Frontend_Dashboard.Components.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMemoryCache();
builder.Services.AddBlazorBootstrap();
builder.Services.AddSingleton<DateService>();
builder.Services.AddScoped<IDetectionDataService, DetectionDataService>();
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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();