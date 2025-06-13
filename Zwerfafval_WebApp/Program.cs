using Zwerfafval_WebApp.Components;
using Zwerfafval_WebApp.Components.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DateService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<IDetectionDataService, DetectionDataService>();
builder.Services.AddScoped<HttpClient>(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri("builder.HostEnvironment.BaseAddress") // 
    };
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
