using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using Infrastructure.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using Polly;
using RecAll.Core.List.Api.Infrastructure;
using RecAll.Core.List.Api.Infrastructure.AutofacModules;
using RecAll.Core.List.Api.Infrastructure.Filters;
using RecAll.Core.List.Infrastructure;
using RecAll.Infrastructure.IntegrationEventLog;
using Serilog;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api;

public static class ProgramExtensions {
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;

    public static void AddCustomCors(this WebApplicationBuilder builder) =>
        builder.Services.AddCors(options => {
            options.AddPolicy("CorsPolicy",
                builder => builder.SetIsOriginAllowed(host => true)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });

    public static void AddCustomServiceProviderFactory(
        this WebApplicationBuilder builder) {
        builder.Host.UseServiceProviderFactory(
            new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => {
            containerBuilder.RegisterModule(new MediatorModule());
            containerBuilder.RegisterModule(new ApplicationModule());
        });
    }

    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder) {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder) {
        var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"];

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration).WriteTo.Console().WriteTo
            .Seq(seqServerUrl).Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) {
        builder.Services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("oauth2",
                new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows {
                        Implicit = new OpenApiOAuthFlow {
                            AuthorizationUrl =
                                new Uri(
                                    $"{builder.Configuration["IdentityServer"]}/connect/authorize"),
                            TokenUrl =
                                new Uri(
                                    $"{builder.Configuration["IdentityServer"]}/connect/token"),
                            Scopes = new Dictionary<string, string> {
                                ["List"] = "List",
                                ["TextList"] = "TextList",
                                ["MaskedTextList"] = "MaskedTextList",

                            }
                        }
                    }
                });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }

    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy()).AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:ListContext"]!,
                name: "ListDb-check", tags: new[] {
                    "ListDb"
                }).AddUrlGroup(
                new Uri(builder.Configuration["TextListHealthCheck"]),
                "TextListHealthCheck", tags: new[] {
                    "TextList"
                }).AddUrlGroup(
                new Uri(builder.Configuration["MaskedTextListHealthCheck"]),
                "MaskedTextListHealthCheck", tags: new[] {
                    "MaskedTextList"
                }).AddUrlGroup(
                new Uri(builder.Configuration["IdentityServerHealthCheck"]),
                "IdentityServerHealthCheck", tags: new[] {
                    "IdentityServer"
                });

    public static void AddCustomDatabase(this WebApplicationBuilder builder) {
        builder.Services.AddDbContext<ListContext>(options => {
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:ListContext"],
                sqlServerOptionsAction => {
                    sqlServerOptionsAction.MigrationsAssembly(
                        typeof(ProgramExtensions).GetTypeInfo().Assembly
                            .GetName().Name);
                    sqlServerOptionsAction.EnableRetryOnFailure(15,
                        TimeSpan.FromSeconds(30), null);
                });
        });

        builder.Services.AddDbContext<IntegrationEventLogContext>(options => {
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:ListContext"],
                sqlServerOptionsAction => {
                    sqlServerOptionsAction.MigrationsAssembly(
                        typeof(ProgramExtensions).GetTypeInfo().Assembly
                            .GetName().Name);
                    sqlServerOptionsAction.EnableRetryOnFailure(15,
                        TimeSpan.FromSeconds(30), null);
                });
        });
    }

    public static void AddCustomIdentityService(
        this WebApplicationBuilder builder) {
        // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
        var identityServerUrl = builder.Configuration["IdentityServer"];
        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.Authority = identityServerUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "List";
            options.TokenValidationParameters.ValidateIssuer = false;
            options.TokenValidationParameters.SignatureValidator = (token, _) => new JsonWebToken(token);
        });
    }

    public static void AddInvalidModelStateResponseFactory(
        this WebApplicationBuilder builder) {
        builder.Services.AddOptions().PostConfigure<ApiBehaviorOptions>(
            options => {
                options.InvalidModelStateResponseFactory = context =>
                    new OkObjectResult(ServiceResult
                        .CreateInvalidParameterResult(
                            new ValidationProblemDetails(context.ModelState)
                                .Errors.Select(p =>
                                    $"{p.Key}: {string.Join(" / ", p.Value)}"))
                        .ToServiceResultViewModel());
            });
    }

    public static void
        AddCustomControllers(this WebApplicationBuilder builder) =>
        builder.Services
            .AddControllers(options =>
                options.Filters.Add(typeof(HttpGlobalExceptionFilter)))
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.IncludeFields = true);

    public static void UseCustomSwagger(this WebApplication app) {
        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.OAuthClientId("ListApiSwaggerUI");
            options.OAuthAppName("ListApiSwaggerUI");
        });
    }

    public static void UserCustomCors(this WebApplication app) {
        app.UseCors("CorsPolicy");
    }

    public static void ApplyDatabaseMigration(this WebApplication app) {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var listContext =
            scope.ServiceProvider.GetRequiredService<ListContext>();
        var listContextSeedLogger = scope.ServiceProvider
            .GetRequiredService<ILogger<ListContextSeed>>();

        retryPolicy.Execute(() => {
            listContext.Database.Migrate();
            new ListContextSeed().SeedAsync(listContext, listContextSeedLogger)
                .Wait();
        });

        var integrationEventLogContext = scope.ServiceProvider
            .GetRequiredService<IntegrationEventLogContext>();
        retryPolicy.Execute(() => {
            integrationEventLogContext.Database.Migrate();
        });
    }

    private static Policy CreateRetryPolicy() {
        return Policy.Handle<Exception>().WaitAndRetryForever(
            sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
            onRetry: (exception, retry, _) => {
                Console.WriteLine(
                    "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                    exception.GetType().Name, exception.Message, retry);
            });
    }
}