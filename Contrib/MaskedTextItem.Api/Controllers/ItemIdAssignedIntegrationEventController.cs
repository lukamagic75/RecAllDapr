using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.MaskedTextItem.Api.IntegrationEvents;
using RecAll.Contrib.MaskedTextItem.Api.Services;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.EventBus;

namespace RecAll.Contrib.MaskedTextItem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemIdAssignedIntegrationEventController {
    private readonly MaskedTextItemContext _maskedTextItemContext;

    private readonly ILogger<ItemIdAssignedIntegrationEventController> _logger;

    public ItemIdAssignedIntegrationEventController(
        MaskedTextItemContext maskedTextItemContext,
        ILogger<ItemIdAssignedIntegrationEventController> logger) {
        _maskedTextItemContext = maskedTextItemContext;
        _logger = logger;
    }

    [Route("itemIdAssigned")]
    [HttpPost]
    [Topic(DaprEventBus.PubSubName, nameof(ItemIdAssignedIntegrationEvent))]
    public async Task HandleAsync(ItemIdAssignedIntegrationEvent @event) {
        if (@event.TypeId != ListType.MaskedText.Id) {
            return;
        }

        _logger.LogInformation(
            "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, ProgramExtensions.AppName, @event);

        var maskedTextItem = await _maskedTextItemContext.MaskedTextItems.FirstOrDefaultAsync(p =>
            p.Id == int.Parse(@event.ContribId));

        if (maskedTextItem is null) {
            _logger.LogWarning("Unknown MaskedTextItem id: {ItemId}", @event.ItemId);
            return;
        }

        maskedTextItem.ItemId = @event.ItemId;
        await _maskedTextItemContext.SaveChangesAsync();

        _logger.LogInformation(
            "----- Integration event handled: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, ProgramExtensions.AppName, @event);
    }
}