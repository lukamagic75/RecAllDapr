using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus.Abstractions;

public interface
    IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent {
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler { }