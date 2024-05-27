using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.MaskedTextItem.Api.Commands;
using RecAll.Contrib.MaskedTextItem.Api.Services;
using RecAll.Contrib.MaskedTextItem.Api.ViewModels;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Contrib.MaskedTextItem.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ItemController {
    private readonly IIdentityService _identityService;
    private readonly MaskedTextItemContext _maskedTextItemContext;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IIdentityService identityService,
        MaskedTextItemContext maskedTextItemContext, ILogger<ItemController> logger) {
        _identityService = identityService;
        _maskedTextItemContext = maskedTextItemContext;
        _logger = logger;
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel<string>>> CreateAsync(
        [FromBody] CreateMaskedTextItemCommand command) {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);

        var maskedTextItem = new Models.MaskedTextItem {
            Content = command.Content,
            MaskedContent = command.MaskedContent,
            UserIdentityGuid = _identityService.GetUserIdentityGuid(),
            IsDeleted = false
        };
        var maskedTextItemEntity = _maskedTextItemContext.Add(maskedTextItem);
        await _maskedTextItemContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult<string>
            .CreateSucceededResult(maskedTextItemEntity.Entity.Id.ToString())
            .ToServiceResultViewModel();
    }

    [Route("update")]
    [HttpPost]
    public async Task<ServiceResultViewModel> UpdateAsync(
        [FromBody] UpdateMaskedTextItemCommand command) {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);

        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var maskedTextItem = await _maskedTextItemContext.MaskedTextItems.FirstOrDefaultAsync(p =>
            p.Id == command.Id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (maskedTextItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的MaskedTextItem {command.Id}");

            return ServiceResult
                .CreateFailedResult($"Unknown MaskedTextItem id: {command.Id}")
                .ToServiceResultViewModel();
        }

        maskedTextItem.Content = command.Content;
        maskedTextItem.MaskedContent = command.MaskedContent;
        await _maskedTextItemContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult.CreateSucceededResult().ToServiceResultViewModel();
    }

    [Route("get/{id}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<MaskedTextItemViewModel>>>
        GetAsync(int id) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var maskedTextItem = await _maskedTextItemContext.MaskedTextItems.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (maskedTextItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的MaskedTextItem {id}");

            return ServiceResult<MaskedTextItemViewModel>
                .CreateFailedResult($"Unknown MaskedTextItem id: {id}")
                .ToServiceResultViewModel();
        }

        return ServiceResult<MaskedTextItemViewModel>
            .CreateSucceededResult(new MaskedTextItemViewModel {
                ItemId = maskedTextItem.ItemId,
                Content = maskedTextItem.Content,
                MaskedContent=maskedTextItem.MaskedContent,
            }).ToServiceResultViewModel();
    }

    [Route("getByItemId/{itemId}")]
    [HttpGet]
    // ServiceResultViewModel<MaskedTextItemViewModel>
    public async Task<ActionResult<ServiceResultViewModel<MaskedTextItemViewModel>>>
        GetByItemId(int itemId) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var maskedTextItem = await _maskedTextItemContext.MaskedTextItems.FirstOrDefaultAsync(p =>
            p.ItemId == itemId && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (maskedTextItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的MaskedTextItem, ItemID: {itemId}");

            return ServiceResult<MaskedTextItemViewModel>
                .CreateFailedResult($"Unknown MaskedTextItem with ItemID: {itemId}")
                .ToServiceResultViewModel();
        }

        return ServiceResult<MaskedTextItemViewModel>
            .CreateSucceededResult(new MaskedTextItemViewModel {
                Id = maskedTextItem.Id,
                ItemId = maskedTextItem.ItemId,
                Content = maskedTextItem.Content,
                MaskedContent=maskedTextItem.MaskedContent
            }).ToServiceResultViewModel();
    }

    [Route("getItems")]
    [HttpPost]
    public async
        Task<ActionResult<
            ServiceResultViewModel<IEnumerable<MaskedTextItemViewModel>>>>
        GetItemsAsync(GetItemsCommand command) {
        var itemIds = command.Ids.ToList();
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var maskedTextItems = await _maskedTextItemContext.MaskedTextItems.Where(p =>
                p.ItemId.HasValue && itemIds.Contains(p.ItemId.Value) &&
                p.UserIdentityGuid == userIdentityGuid && !p.IsDeleted)
            .ToListAsync();

        if (maskedTextItems.Count != itemIds.Count) {
            var missingIds = string.Join(",",
                itemIds.Except(maskedTextItems.Select(p => p.ItemId.Value))
                    .Select(p => p.ToString()));

            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的MaskedTextItem {missingIds}");

            return ServiceResult<IEnumerable<MaskedTextItemViewModel>>
                .CreateFailedResult($"Unknown Item id: {missingIds}")
                .ToServiceResultViewModel();
        }

        maskedTextItems.Sort((x, y) =>
            itemIds.IndexOf(x.ItemId.Value) - itemIds.IndexOf(y.ItemId.Value));

        return ServiceResult<IEnumerable<MaskedTextItemViewModel>>
            .CreateSucceededResult(maskedTextItems.Select(p => new MaskedTextItemViewModel {
                Id = p.Id, ItemId = p.ItemId, Content = p.Content, MaskedContent=p.MaskedContent
            })).ToServiceResultViewModel();
    }
}