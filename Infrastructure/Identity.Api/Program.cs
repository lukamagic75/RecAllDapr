using HealthChecks.UI.Client;
using RecAll.Infrastructure.Identity.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddCustomHealthChecks();
builder.Services.AddDaprClient();
builder.Services.AddRazorPages();
builder.AddCustomIdentityServer();

var app = builder.Build();

app.UseIdentityServer();
app.UseCookiePolicy(
    new CookiePolicyOptions {
        MinimumSameSitePolicy = SameSiteMode.Lax
    });
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapCustomHealthChecks(
    responseWriter: UIResponseWriter.WriteHealthCheckUIResponse);

await app.ApplyDatabaseMigrationAsync();
app.Run();