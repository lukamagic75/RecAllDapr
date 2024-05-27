using RecAll.Core.List.Domain.Events;
using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels.ItemAggregate;

public class Item : Entity, IAggregateRoot {
    private int _typeId;

    public ListType Type { get; private set; }

    private int _setId;

    public int SetId => _setId;

    private string _contribId;

    public string ContribId => _contribId;

    private string _userIdentityGuid;

    public string UserIdentityGuid => _userIdentityGuid;

    private bool _isDeleted;

    public bool IsDeleted => _isDeleted;

    private Item() { }

    public Item(int typeId, int setId, string contribId,
        string userIdentityGuid) : this() {
        _typeId = typeId;
        _setId = setId;
        _contribId = contribId;
        _userIdentityGuid = userIdentityGuid;

        var itemCreatedDomainEvent = new ItemCreatedDomainEvent(this);
        AddDomainEvent(itemCreatedDomainEvent);
    }

    public void SetSetId(int setId) {
        if (_isDeleted) {
            ThrowDeletedException();
        }

        _setId = setId;
    }

    public void SetDeleted() {
        if (_isDeleted) {
            ThrowDeletedException();
        }

        _isDeleted = true;

        var itemDeletedDomainEvent = new ItemDeletedDomainEvent(this);
        AddDomainEvent(itemDeletedDomainEvent);
    }

    private void ThrowDeletedException() =>
        throw new ListDomainException("项目已删除。");
}