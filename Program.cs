using projekt.Components;
using projekt.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register our CsvService
builder.Services.AddScoped<CsvService>();

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

// Log the WebRootPath
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"WebRootPath: {app.Environment.WebRootPath}");

// Ensure data directory exists
string dataDir = Path.Combine(app.Environment.WebRootPath, "data");
if (!Directory.Exists(dataDir))
{
    Directory.CreateDirectory(dataDir);
    logger.LogInformation($"Created data directory: {dataDir}");
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();