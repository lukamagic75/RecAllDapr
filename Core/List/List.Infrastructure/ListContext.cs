using System.Data;
using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using RecAll.Core.List.Infrastructure.EntityConfigurations;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;
using RecAll.Infrastructure.Ddd.Infrastructure;

namespace RecAll.Core.List.Infrastructure;

public class ListContext : DbContext, IUnitOfWork {
    public const string DefaultSchema = "list";

    public DbSet<Domain.AggregateModels.ListAggregate.List> Lists { get; set; }

    public DbSet<Set> Sets { get; set; }
    
    public DbSet<Item> Items { get; set; }
    
    public DbSet<ListType> ListTypes { get; set; }

    private readonly IMediator _mediator;

    private IDbContextTransaction _currentTransaction;

    public IDbContextTransaction CurrentTransaction => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    public ListContext(DbContextOptions<ListContext> options) :
        base(options) { }

    public ListContext(DbContextOptions<ListContext> options,
        IMediator mediator) : base(options) {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));

        Debug.WriteLine($"TaskContext::ctor -> {GetHashCode()}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new ListTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ListConfiguration());
        modelBuilder.ApplyConfiguration(new SetConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());

    }

    public async Task<bool> SaveEntitiesAsync(
        CancellationToken cancellationToken = default) {
        await _mediator.DispatchDomainEventsAsync(this);
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() {
        if (_currentTransaction != null) {
            return null;
        }

        return _currentTransaction =
            await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task
        CommitTransactionAsync(IDbContextTransaction transaction) {
        if (transaction is null) {
            throw new ArgumentNullException(nameof(transaction));
        }

        if (transaction != _currentTransaction) {
            throw new InvalidOperationException(
                $"Transaction {transaction.TransactionId} is not current");
        }

        try {
            await SaveChangesAsync();
            transaction.Commit();
        } catch {
            RollbackTransaction();
            throw;
        } finally {
            if (_currentTransaction != null) {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction() {
        try {
            _currentTransaction?.Rollback();
        } finally {
            if (_currentTransaction != null) {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}

public class
    ListContextDesignFactory : IDesignTimeDbContextFactory<ListContext> {
    public ListContext CreateDbContext(string[] args) =>
        new(
            new DbContextOptionsBuilder<ListContext>()
                .UseSqlServer(
                    "Server=.;Initial Catalog=RecAll.ListDb;Integrated Security=true")
                .Options, new NoMediator());
}