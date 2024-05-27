using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class SetController {
    private readonly IMediator _mediator;
    private readonly ISetQueryService _setQueryService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<SetController> _logger;

    public SetController(IMediator mediator, ISetQueryService setQueryService,
        IIdentityService identityService, ILogger<SetController> logger) {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
        _setQueryService = setQueryService ??
            throw new ArgumentNullException(nameof(setQueryService));
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Route("list")]
    [HttpGet]
    public async
        Task<ActionResult<
            ServiceResultViewModel<(IEnumerable<SetViewModel>, int)>>>
        ListAsync([Required] int listId, int skip = 0, int take = 20) =>
        ServiceResult<(IEnumerable<SetViewModel>, int)>
            .CreateSucceededResult(await _setQueryService.ListAsync(listId,
                skip, take, _identityService.GetUserIdentityGuid()))
            .ToServiceResultViewModel();

    [Route("get/{id}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<SetViewModel>>>
        GetAsync([Required] int id) {
        var setViewModel = await _setQueryService.GetAsync(id,
            _identityService.GetUserIdentityGuid());

        if (setViewModel is null) {
            _logger.LogWarning(
                $"用户{_identityService.GetUserIdentityGuid()}尝试查看已删除、不存在或不属于自己的Set {id}");
            return ServiceResult<SetViewModel>
                .CreateFailedResult(ErrorMessage
                    .NotFoundOrDeletedOrIdentityMismatch)
                .ToServiceResultViewModel();
        }

        return ServiceResult<SetViewModel>.CreateSucceededResult(setViewModel)
            .ToServiceResultViewModel();
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> CreateAsync(
        [FromBody] CreateSetCommand command) {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);

        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }

    [Route("update")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> UpdateAsync(
        [FromBody] UpdateSetCommand command) {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);

        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }

    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> DeleteAsync(
        [FromBody] DeleteSetCommand command) {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);

        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }
}