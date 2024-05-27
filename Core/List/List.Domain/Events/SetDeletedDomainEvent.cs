using MediatR;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;

namespace RecAll.Core.List.Domain.Events;

public class SetDeletedDomainEvent : INotification {
    public Set Set { get; set; }

    public SetDeletedDomainEvent(Set set) {
        Set = set;
    }
}