using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RemoteListe;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ActiveDirectoryService>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<ActiveDirectoryService>());

builder.Logging.ClearProviders();
builder.Services.AddLogging(o => o
    .SetMinimumLevel(LogLevel.Trace)
    .AddDebug()
    .AddConsole()
);
var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("C:\\temp\\remote_liste_logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//var logger = LoggerFactory.Create(config =>
//{
//    config.AddConsole();
//}).CreateLogger("Program");

//logger.LogInformation("Starting remote list");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
