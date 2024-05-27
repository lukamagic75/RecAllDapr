using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.IntegrationEventLog.Models;

public class IntegrationEventLogEntry {
    public Guid EventId { get; private set; }

    public string EventTypeName { get; private set; }

    [NotMapped] public string EventTypeShortName => EventTypeName.Split('.')?.Last();

    [NotMapped] public IntegrationEvent IntegrationEvent { get; private set; }

    public EventState State { get; set; }

    public int TimesSent { get; set; }

    public DateTime CreatedTime { get; private set; }

    public string ContentJson { get; private set; }

    public string TransactionId { get; private set; }

    public IntegrationEventLogEntry DeserializeIntegrationEvent(Type type) {
        IntegrationEvent = JsonSerializer.Deserialize(ContentJson, type,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) as
            IntegrationEvent;
        return this;
    }

    private IntegrationEventLogEntry() { }

    public IntegrationEventLogEntry(IntegrationEvent integrationEvent,
        Guid transactionId) {
        EventId = integrationEvent.Id;
        CreatedTime = integrationEvent.CreatedTime;
        EventTypeName = integrationEvent.GetType().FullName;
        ContentJson = JsonSerializer.Serialize(integrationEvent,
            integrationEvent.GetType(),
            new JsonSerializerOptions {
                PropertyNameCaseInsensitive = false
            });
        State = EventState.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId.ToString();
    }
}