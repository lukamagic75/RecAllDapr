using MediatR;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class UpdateSetCommandHandler : IRequestHandler<UpdateSetCommand,
    ServiceResult> {
    private readonly IIdentityService _identityService;
    private readonly ISetRepository _setRepository;
    private readonly ILogger<UpdateSetCommandHandler> _logger;

    public UpdateSetCommandHandler(IIdentityService identityService,
        ISetRepository setRepository, ILogger<UpdateSetCommandHandler> logger) {
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _setRepository = setRepository ??
            throw new ArgumentNullException(nameof(setRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult> Handle(UpdateSetCommand command,
        CancellationToken cancellationToken) {
        (await _setRepository.GetAsync(command.Id,
            _identityService.GetUserIdentityGuid())).SetName(command.Name);
        return await _setRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}