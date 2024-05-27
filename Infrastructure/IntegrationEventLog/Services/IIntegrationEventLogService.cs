using Microsoft.EntityFrameworkCore.Storage;
using RecAll.Infrastructure.EventBus.Events;
using RecAll.Infrastructure.IntegrationEventLog.Models;

namespace RecAll.Infrastructure.IntegrationEventLog.Services;

public interface IIntegrationEventLogService {
    Task<IEnumerable<IntegrationEventLogEntry>>
        RetrieveEventLogsPendingToPublishAsync(Guid transactionId);

    Task SaveEventAsync(IntegrationEvent integrationEvent,
        IDbContextTransaction transaction);

    Task MarkEventAsPublishedAsync(Guid eventId);

    Task MarkEventAsInProgressAsync(Guid eventId);

    Task MarkEventAsFailedAsync(Guid eventId);
}