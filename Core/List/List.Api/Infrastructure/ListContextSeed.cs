using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Core.List.Infrastructure;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Api.Infrastructure;

public class ListContextSeed {
    public async Task SeedAsync(ListContext context,
        ILogger<ListContextSeed> logger, int retry = 0) {
        var retryForAvailability = retry;
        try {
            if (!context.ListTypes.Any()) {
                context.ListTypes.AddRange(Enumeration.GetAll<ListType>());
                await context.SaveChangesAsync();
            }
        } catch (Exception e) {
            if (retryForAvailability < 10) {
                retryForAvailability++;
                logger.LogError(e,
                    "EXCEPTION ERROR while migrating {DbContextName}",
                    nameof(ListContext));
                await SeedAsync(context, logger, retryForAvailability);
            }
        }
    }
}