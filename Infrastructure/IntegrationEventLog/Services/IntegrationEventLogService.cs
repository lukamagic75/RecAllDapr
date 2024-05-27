using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RecAll.Infrastructure.EventBus.Events;
using RecAll.Infrastructure.IntegrationEventLog.Models;

namespace RecAll.Infrastructure.IntegrationEventLog.Services;

public class IntegrationEventLogService : IIntegrationEventLogService,
    IDisposable {
    private readonly IntegrationEventLogContext _integrationEventLogContext;
    private readonly DbConnection _dbConnection;
    private readonly List<Type> _eventTypes;
    private volatile bool _disposedValue;

    public IntegrationEventLogService(DbConnection dbConnection) {
        _dbConnection = dbConnection ??
            throw new ArgumentNullException(nameof(dbConnection));
        _integrationEventLogContext = new IntegrationEventLogContext(
            new DbContextOptionsBuilder<IntegrationEventLogContext>()
                .UseSqlServer(_dbConnection).Options);
        _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
            .GetTypes().Where(p => p.Name.EndsWith(nameof(IntegrationEvent)))
            .ToList();
    }

    public async Task<IEnumerable<IntegrationEventLogEntry>>
        RetrieveEventLogsPendingToPublishAsync(Guid transactionId) {
        var result = await _integrationEventLogContext.IntegrationEventLogs
            .Where(p =>
                p.TransactionId == transactionId.ToString() &&
                p.State == EventState.NotPublished).ToListAsync();

        return result is null || !result.Any()
            ? new List<IntegrationEventLogEntry>()
            : result.OrderBy(p => p.CreatedTime).Select(p =>
                p.DeserializeIntegrationEvent(
                    _eventTypes.Find(q => q.Name == p.EventTypeShortName)));
    }

    public Task SaveEventAsync(IntegrationEvent integrationEvent,
        IDbContextTransaction transaction) {
        if (transaction is null) {
            throw new ArgumentNullException(nameof(transaction));
        }

        var eventLogEntry = new IntegrationEventLogEntry(integrationEvent,
            transaction.TransactionId);

        _integrationEventLogContext.Database.UseTransaction(
            transaction.GetDbTransaction());
        _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

        return _integrationEventLogContext.SaveChangesAsync();
    }

    public Task MarkEventAsPublishedAsync(Guid eventId) =>
        UpdateEventStatus(eventId, EventState.Published);

    public Task MarkEventAsInProgressAsync(Guid eventId) =>
        UpdateEventStatus(eventId, EventState.InProcess);

    public Task MarkEventAsFailedAsync(Guid eventId) =>
        UpdateEventStatus(eventId, EventState.PublishedFailed);

    private Task UpdateEventStatus(Guid eventId, EventState status) {
        var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs
            .Single(p => p.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventState.InProcess) {
            eventLogEntry.TimesSent++;
        }

        _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);
        return _integrationEventLogContext.SaveChangesAsync();
    }


    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposedValue) {
            return;
        }

        if (disposing) {
            _integrationEventLogContext?.Dispose();
        }

        _disposedValue = true;
    }
}