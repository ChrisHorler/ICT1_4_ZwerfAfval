using Frontend_Dashboard.Components;
using Frontend_Dashboard.Components.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DateService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<IDetectionDataService, DetectionDataService>();
builder.Services.AddHttpClient("BackendApi", client =>
{
    var baseUrl = builder.Configuration["API_BASE_URL"];
    if (string.IsNullOrEmpty(baseUrl))
        throw new Exception("API_BASE_URL enviromental variable not detected");

    client.BaseAddress = new Uri(baseUrl);
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