namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

public interface IRepository<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot {
    IUnitOfWork UnitOfWork { get; }
}