using MediatR;

namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

public abstract class Entity {
    protected int _id;

    public virtual int Id {
        get => _id;
        protected set => _id = value;
    }

    private List<INotification> _domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents =>
        _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent) {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvnet(INotification domainEvent) =>
        _domainEvents?.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents?.Clear();

    public bool IsTransient => Id == default;

    protected int? _requestedHashCode;

    public override bool Equals(object obj) {
        if (obj is not Entity entity) {
            return false;
        }

        if (ReferenceEquals(this, entity)) {
            return true;
        }

        if (GetType() != entity.GetType()) {
            return false;
        }

        if (entity.IsTransient || IsTransient) {
            return false;
        }

        return entity.Id == Id;
    }

    public override int GetHashCode() {
        if (IsTransient) {
            return base.GetHashCode();
        }

        _requestedHashCode ??= Id.GetHashCode() ^ 31;
        return _requestedHashCode.Value;
    }

    public static bool operator ==(Entity left, Entity right) =>
        Equals(left, null) ? Equals(right, null) : left.Equals(right);

    public static bool operator !=(Entity left, Entity right) =>
        !(left == right);
}