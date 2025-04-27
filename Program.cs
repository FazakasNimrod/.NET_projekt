using projekt.Components;
using projekt.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;

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

// Configure request body size limits for file uploads
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

// Configure form options for larger files
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
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

// Log the WebRootPath
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"WebRootPath: {app.Environment.WebRootPath}");

// Set up sample data for development
try
{
    logger.LogInformation("Setting up CSV sample data");
    CsvSetupHelper.EnsureDataDirectoryAndSampleFiles(app.Environment);
    logger.LogInformation("Sample data setup completed");
}
catch (Exception ex)
{
    logger.LogError(ex, "Error setting up sample data");
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();