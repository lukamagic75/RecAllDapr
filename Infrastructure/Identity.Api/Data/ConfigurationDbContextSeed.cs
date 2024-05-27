using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;

namespace RecAll.Infrastructure.Identity.Api.Data;

public class ConfigurationDbContextSeed {
    public async Task SeedAsync(ConfigurationDbContext context,
        IConfiguration configuration) {
        var clientUrlDict = new Dictionary<string, string> {
            ["ListApi"] = configuration.GetValue<string>("ListApi"),
            ["TextListApi"] = configuration.GetValue<string>("TextListApi"),
            ["MaskedTextListApi"] = configuration.GetValue<string>("MaskedTextListApi")

        };

        if (!context.Clients.Any()) {
            foreach (var client in Config.GetClients(clientUrlDict)) {
                context.Clients.Add(client.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.IdentityResources.Any()) {
            foreach (var resource in Config.IdentityResources) {
                context.IdentityResources.Add(resource.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any()) {
            foreach (var api in Config.ApiScopes) {
                context.ApiScopes.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any()) {
            foreach (var apiResource in Config.ApiResources) {
                context.ApiResources.Add(apiResource.ToEntity());
            }

            await context.SaveChangesAsync();
        }
    }
}