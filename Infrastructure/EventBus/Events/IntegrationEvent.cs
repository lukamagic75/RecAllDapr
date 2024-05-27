using System.Text.Json.Serialization;

namespace RecAll.Infrastructure.EventBus.Events;

public record IntegrationEvent {
    [JsonInclude] public Guid Id { get; private init; }

    [JsonInclude] public DateTime CreatedTime { get; private init; }

    public IntegrationEvent() {
        Id = Guid.NewGuid();
        CreatedTime = DateTime.Now;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createdTime) {
        Id = id;
        CreatedTime = createdTime;
    }
}