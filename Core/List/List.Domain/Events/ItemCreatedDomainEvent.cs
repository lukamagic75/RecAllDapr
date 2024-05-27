using MediatR;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;

namespace RecAll.Core.List.Domain.Events;

public class ItemCreatedDomainEvent : INotification
{
    public Item Item { get; }

    public ItemCreatedDomainEvent(Item item)
    {
        Item = item;
    }
}