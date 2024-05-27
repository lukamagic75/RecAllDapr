using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Core.List.Api.Application.IntegrationEvents;

public interface IListIntegrationEventService {
    Task AddAndSaveEventAsync(IntegrationEvent integrationEvent);

    Task PublishEventsAsync(Guid transactionId);
}