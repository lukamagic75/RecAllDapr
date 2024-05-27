namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

public interface IUnitOfWork : IDisposable {
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}