using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Core.List.Api.Application.IntegrationEvents;

public record ItemIdAssignedIntegrationEvent(
    int typeId,
    string contribId,
    int itemId) : IntegrationEvent;