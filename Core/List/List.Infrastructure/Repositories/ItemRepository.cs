using Microsoft.EntityFrameworkCore;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly ListContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public Item Add(Item item) => _context.Items.Add(item).Entity;

    public ItemRepository(ListContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Update(Item item) =>
        _context.Entry(item).State = EntityState.Modified;

    public async Task<Item> GetAsync(int itemId, string userIdentityGuid)
    {
        var item =
            await _context.Items.FirstOrDefaultAsync(p =>
                p.Id == itemId && p.UserIdentityGuid == userIdentityGuid &&
                !p.IsDeleted) ?? _context.Items.Local.FirstOrDefault(p =>
                p.Id == itemId && p.UserIdentityGuid == userIdentityGuid &&
                !p.IsDeleted);

        if (item is null)
        {
            return null;
        }

        await _context.Entry(item).Reference(p => p.Type).LoadAsync();
        return item;
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(IEnumerable<int> itemIds,
        string userIdentityGuid)
    {
        var items =
            (await _context.Items.Where(p =>
                    itemIds.Contains(p.Id) &&
                    p.UserIdentityGuid == userIdentityGuid && !p.IsDeleted)
                .ToListAsync()).UnionBy(
                _context.Items.Local.Where(p =>
                        itemIds.Contains(p.Id) &&
                        p.UserIdentityGuid == userIdentityGuid && !p.IsDeleted)
                    .ToList(), p => p.Id);

        await Task.WhenAll(items.Select(p =>
            _context.Entry(p).Reference(p => p.Type).LoadAsync()));
        return items;
    }
}