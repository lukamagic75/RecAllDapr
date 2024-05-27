var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecksUI().AddInMemoryStorage();

var app = builder.Build();

app.UseRouting().UseEndpoints(config => config.MapHealthChecksUI());
app.MapGet("/", () => Results.LocalRedirect("~/healthchecks-ui"));

app.Run();