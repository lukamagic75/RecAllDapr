using System.Reflection;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Services;
using Identity.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RecAll.Infrastructure.Identity.Api.Data;
using RecAll.Infrastructure.Identity.Api.Models;
using RecAll.Infrastructure.Identity.Api.Services;

namespace RecAll.Infrastructure.Identity.Api;

public static class ProgramExtensions {
    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder) {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }

    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy()).AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:IdentityContext"]!,
                name: "TextListDb-check", tags: new[] {
                    "TextListDb"
                });

    public static void AddCustomIdentityServer(
        this WebApplicationBuilder builder) {
        builder.Services
            .AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
        builder.Services.AddTransient<IRedirectService, RedirectService>();

        var connectionString =
            builder.Configuration["ConnectionStrings:IdentityContext"];
        var migrationAssemblyString = typeof(ProgramExtensions).GetTypeInfo()
            .Assembly.GetName().Name;

        builder.Services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlServer(connectionString, sqlServerOptionsAction => {
                sqlServerOptionsAction.MigrationsAssembly(
                    migrationAssemblyString);
                sqlServerOptionsAction.EnableRetryOnFailure(15,
                    TimeSpan.FromSeconds(30), null);
            });
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(x => {
                x.IssuerUri = "null";
                x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            }).AddAspNetIdentity<ApplicationUser>().AddConfigurationStore(
                options => {
                    options.ConfigureDbContext = optionsBuilder =>
                        optionsBuilder.UseSqlServer(connectionString, sql => {
                            sql.EnableRetryOnFailure(15,
                                TimeSpan.FromSeconds(30),
                                null);
                            sql.MigrationsAssembly(migrationAssemblyString);
                        });
                }).AddOperationalStore(options => {
                options.ConfigureDbContext = optionsBuilder =>
                    optionsBuilder.UseSqlServer(connectionString, sql => {
                        sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30),
                            null);
                        sql.MigrationsAssembly(migrationAssemblyString);
                    });
            }).AddDeveloperSigningCredential().Services
            .AddTransient<IProfileService, ProfileService>();
    }

    public static async Task ApplyDatabaseMigrationAsync(
        this WebApplication app) {
        using var serviceScope = app.Services.CreateScope();
        var persistedGrantDbContext = serviceScope.ServiceProvider
            .GetRequiredService<PersistedGrantDbContext>();
        var configurationDbContext = serviceScope.ServiceProvider
            .GetRequiredService<ConfigurationDbContext>();
        persistedGrantDbContext.Database.Migrate();
        configurationDbContext.Database.Migrate();
        await new ConfigurationDbContextSeed().SeedAsync(configurationDbContext,
            app.Configuration);

        var applicationDbContext = serviceScope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        applicationDbContext.Database.Migrate();
        await new ApplicationDbContextSeed().SeedAsync(applicationDbContext);
    }
}