using MediatR;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateSetCommandHandler : IRequestHandler<CreateSetCommand,
    ServiceResult> {
    private readonly IListQueryService _listQueryService;
    private readonly IIdentityService _identityService;
    private readonly ISetRepository _setRepository;

    public CreateSetCommandHandler(IListQueryService listQueryService,
        IIdentityService identityService, ISetRepository setRepository) {
        _listQueryService = listQueryService ??
            throw new ArgumentNullException(nameof(listQueryService));
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _setRepository = setRepository ??
            throw new ArgumentNullException(nameof(setRepository));
    }

    public async Task<ServiceResult> Handle(CreateSetCommand command,
        CancellationToken cancellationToken) {
        var list = await _listQueryService.GetAsync(command.ListId,
            _identityService.GetUserIdentityGuid());
        _setRepository.Add(new Set(command.Name, list.TypeId, list.Id,
            _identityService.GetUserIdentityGuid()));
        return await _setRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}