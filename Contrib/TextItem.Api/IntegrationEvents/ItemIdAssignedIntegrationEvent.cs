using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Contrib.TextItem.Api.IntegrationEvents;

public record ItemIdAssignedIntegrationEvent(
    int ItemId,
    int TypeId,
    string ContribId) : IntegrationEvent;