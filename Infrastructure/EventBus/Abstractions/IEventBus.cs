using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus.Abstractions;

public interface IEventBus {
    Task PublishAsync(IntegrationEvent @event);
}