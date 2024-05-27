using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels.SetAggregate;

public interface ISetRepository : IRepository<Set> {
    Set Add(Set set);

    Task<Set> GetAsync(int setId, string userIdentityGuid);
}