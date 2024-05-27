using RecAll.Core.List.Domain.Events;
using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels.SetAggregate;

public class Set : Entity, IAggregateRoot {
    private string _name;

    private int _typeId;

    public ListType Type { get; private set; }

    private int _listId;

    public int ListId => _listId;

    private string _userIdentityGuid;

    public string UserIdentityGuid => _userIdentityGuid;

    private bool _isDeleted;

    public bool IsDeleted => _isDeleted;

    private Set() { }

    public Set(string name, int typeId, int listId, string userIdentityGuid) :
        this() {
        _name = name;
        _typeId = typeId;
        _listId = listId;
        _userIdentityGuid = userIdentityGuid;

        var setCreatedDomainEvent = new SetCreatedDomainEvent(this);
        AddDomainEvent(setCreatedDomainEvent);
    }

    public void SetDeleted() {
        if (_isDeleted) {
            ThrowDeletedException();
        }

        _isDeleted = true;

        var setDeletedDomainEvent = new SetDeletedDomainEvent(this);
        AddDomainEvent(setDeletedDomainEvent);
    }

    public void SetName(string name) {
        if (_isDeleted) {
            ThrowDeletedException();
        }

        _name = name;
    }

    public void SetListId(int listId) {
        if (_isDeleted) {
            ThrowDeletedException();
        }

        _listId = listId;
    }

    private void ThrowDeletedException() =>
        throw new ListDomainException("集合已删除。");
}