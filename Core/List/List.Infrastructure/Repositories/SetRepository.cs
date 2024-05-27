using Microsoft.EntityFrameworkCore;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Infrastructure.Repositories;

public class SetRepository : ISetRepository {
    private readonly ListContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public SetRepository(ListContext context) {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Set Add(Set set) => _context.Sets.Add(set).Entity;

    public void Update(Set set) =>
        _context.Entry(set).State = EntityState.Modified;

    public async Task<Set> GetAsync(int setId, string userIdentityGuid) {
        var set =
            await _context.Sets.FirstOrDefaultAsync(p =>
                p.Id == setId && p.UserIdentityGuid == userIdentityGuid &&
                !p.IsDeleted) ?? _context.Sets.Local.FirstOrDefault(p =>
                p.Id == setId && p.UserIdentityGuid == userIdentityGuid &&
                !p.IsDeleted);

        if (set is null) {
            return null;
        }

        await _context.Entry(set).Reference(p => p.Type).LoadAsync();
        return set;
    }
}