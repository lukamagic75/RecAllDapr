using MediatR;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;

namespace RecAll.Core.List.Domain.Events;

public class SetCreatedDomainEvent : INotification {
    public Set Set { get; }

    public SetCreatedDomainEvent(Set set) {
        Set = set;
    }
}