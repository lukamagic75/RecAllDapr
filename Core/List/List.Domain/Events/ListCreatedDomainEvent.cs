using MediatR;

namespace RecAll.Core.List.Domain.Events;

public class ListCreatedDomainEvent : INotification {
    public AggregateModels.ListAggregate.List List { get; set; }

    public ListCreatedDomainEvent(AggregateModels.ListAggregate.List list) {
        List = list;
    }
}