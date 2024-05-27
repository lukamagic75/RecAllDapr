using HealthChecks.UI.Client;
using Infrastructure.Api;
using Infrastructure.Api.HttpClient;
using RecAll.Core.List.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomCors();
builder.AddCustomConfiguration();
builder.AddCustomServiceProviderFactory();
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomHttpClient();
builder.AddCustomDatabase();
builder.AddCustomIdentityService();
builder.AddInvalidModelStateResponseFactory();

builder.Services.AddDaprClient();
builder.AddCustomControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

app.UserCustomCors();

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapControllers();
app.MapCustomHealthChecks(
    responseWriter: UIResponseWriter.WriteHealthCheckUIResponse);
app.ApplyDatabaseMigration();

app.Run();