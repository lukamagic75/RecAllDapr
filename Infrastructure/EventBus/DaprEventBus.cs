using Dapr.Client;
using Microsoft.Extensions.Logging;
using RecAll.Infrastructure.EventBus.Abstractions;
using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus;

public class DaprEventBus : IEventBus {
    public const string PubSubName = "recall-pubsub";

    private readonly DaprClient _dapr;
    private readonly ILogger _logger;

    public DaprEventBus(DaprClient dapr, ILogger<DaprEventBus> logger) {
        _dapr = dapr;
        _logger = logger;
    }

    public async Task PublishAsync(IntegrationEvent @event) {
        var topicName = @event.GetType().Name;
        // ItemDeletedIntegrationEvent : IntegrationEvent
        // "ItemDeletedIntegrationEvent"

        _logger.LogInformation(
            "Publishing event {@Event} to {PubsubName}.{TopicName}", @event,
            PubSubName, topicName);

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(PubSubName, topicName, (object)@event);
    }
}