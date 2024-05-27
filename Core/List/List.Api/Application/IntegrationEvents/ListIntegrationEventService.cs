using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using RecAll.Core.List.Infrastructure;
using RecAll.Infrastructure.EventBus.Abstractions;
using RecAll.Infrastructure.EventBus.Events;
using RecAll.Infrastructure.IntegrationEventLog.Services;

namespace RecAll.Core.List.Api.Application.IntegrationEvents;

public class ListIntegrationEventService : IListIntegrationEventService {
    private readonly Func<DbConnection, IIntegrationEventLogService>
        _integrationEventLogServiceFactory;

    private readonly IEventBus _eventBus;

    private readonly ListContext _listContext;

    private readonly IIntegrationEventLogService _integrationEventLogService;

    private ILogger<ListIntegrationEventService> _logger;

    public ListIntegrationEventService(
        Func<DbConnection, IIntegrationEventLogService>
            integrationEventLogServiceFactory, IEventBus eventBus,
        ListContext listContext, ILogger<ListIntegrationEventService> logger) {
        _integrationEventLogServiceFactory =
            integrationEventLogServiceFactory ??
            throw new ArgumentNullException(
                nameof(integrationEventLogServiceFactory));
        _eventBus = eventBus ??
            throw new ArgumentNullException(nameof(eventBus));
        _listContext = listContext ??
            throw new ArgumentNullException(nameof(eventBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _integrationEventLogService =
            _integrationEventLogServiceFactory(_listContext.Database
                .GetDbConnection());
    }

    public async Task PublishEventsAsync(Guid transactionId) {
        var pendingLogEvents = 
            await _integrationEventLogService
            .RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (var logEvent in pendingLogEvents) {
            _logger.LogInformation(
                "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                logEvent.EventId, ProgramExtensions.AppName,
                logEvent.IntegrationEvent);

            try {
                await _integrationEventLogService.MarkEventAsInProgressAsync(
                    logEvent.EventId);
                await _eventBus.PublishAsync(logEvent.IntegrationEvent);
                await _integrationEventLogService.MarkEventAsPublishedAsync(
                    logEvent.EventId);
            } catch (Exception e) {
                _logger.LogError(e,
                    "ERROR publishing integration event: {IntegrationEventId} from {AppName}",
                    logEvent.EventId, ProgramExtensions.AppName);
                await _integrationEventLogService.MarkEventAsFailedAsync(
                    logEvent.EventId);
            }
        }
    }

    public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent) {
        _logger.LogInformation(
            "----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})",
            integrationEvent.Id, integrationEvent);
        await _integrationEventLogService.SaveEventAsync(integrationEvent,
            _listContext.CurrentTransaction);
    }
}