using Ecmanage.eProcessor.Services.Fetch;
using Ecmanage.eProcessor.Services.Fetch.Fetch.Infrastructure.Data;

var appName = "Fetch CA API";

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

app.UseCloudEvents();

app.UseExceptionHandler(options => { });

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapControllers();
app.MapSubscribeHandler();
app.MapCustomHealthChecks("/hc", "/liveness", UIResponseWriter.WriteHealthCheckUIResponse);

try
{
    app.Logger.LogInformation("Applying database migration ({ApplicationName})...", appName);

    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
}