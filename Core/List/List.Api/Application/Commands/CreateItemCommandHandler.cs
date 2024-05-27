using System.Text.Json;
using Infrastructure.Api.HttpClient;
using MediatR;
using RecAll.Core.List.Api.Application.IntegrationEvents;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand,
    ServiceResult> {
    private ISetQueryService _setQueryService;
    private readonly IIdentityService _identityService;
    private readonly IContribUrlService _contribUrlService;
    private readonly HttpClient _httpClient;
    private readonly IItemRepository _itemRepository;
    private readonly IListIntegrationEventService _listIntegrationEventService;

    public CreateItemCommandHandler(ISetQueryService setQueryService,
        IIdentityService identityService, IItemRepository itemRepository,
        IContribUrlService contribUrlService,
        IHttpClientFactory httpClientFactory,
        IListIntegrationEventService listIntegrationEventService) {
        _setQueryService = setQueryService ??
            throw new ArgumentNullException(nameof(setQueryService));
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _itemRepository = itemRepository ??
            throw new ArgumentNullException(nameof(itemRepository));
        _contribUrlService = contribUrlService;
        _httpClient = httpClientFactory.CreateDefaultClient();
        _listIntegrationEventService = listIntegrationEventService ??
            throw new ArgumentNullException(
                nameof(listIntegrationEventService));
    }

    public async Task<ServiceResult> Handle(CreateItemCommand command,
        CancellationToken cancellationToken) {
        var set = await _setQueryService.GetAsync(command.SetId,
            _identityService.GetUserIdentityGuid());
        var contribUrl =
            $"{_contribUrlService.GetContribUrl(set.TypeId)}/Item/create";

        var jsonContent = JsonContent.Create(command.CreateContribJson,
            options: new JsonSerializerOptions {
                PropertyNameCaseInsensitive = false
            });

        HttpResponseMessage response;
        try {
            response = await _httpClient.PostAsync(contribUrl, jsonContent,
                cancellationToken);
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            return ServiceResult.CreateExceptionResult(e,
                $"访问Contrib Url时发生错误。TypeId: {set.TypeId}, ContribUrl: {contribUrl}");
        }

        var responseJson =
            await response.Content.ReadAsStringAsync(cancellationToken);
        var contribResult = JsonSerializer
            .Deserialize<ServiceResultViewModel<string>>(responseJson,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }).ToServiceResult();

        if (contribResult.Status != ServiceResultStatus.Succeeded) {
            return contribResult;
        }

        var item = _itemRepository.Add(new Item(set.TypeId, command.SetId,
            contribResult.Result, _identityService.GetUserIdentityGuid()));
        var saved =
            await _itemRepository.UnitOfWork.SaveEntitiesAsync(
                cancellationToken);

        if (!saved) {
            return ServiceResult.CreateFailedResult();
        }

        var itemIdAssignedIntegrationEvent =
            new ItemIdAssignedIntegrationEvent(set.TypeId, contribResult.Result,
                item.Id);
        await _listIntegrationEventService.AddAndSaveEventAsync(
            itemIdAssignedIntegrationEvent);

        return await _itemRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}