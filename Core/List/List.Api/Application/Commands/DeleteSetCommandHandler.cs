using MediatR;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class DeleteSetCommandHandler : IRequestHandler<DeleteSetCommand,
    ServiceResult> {
    private readonly IIdentityService _identityService;
    private readonly ISetRepository _setRepository;
    private readonly ILogger<DeleteSetCommandHandler> _logger;

    public DeleteSetCommandHandler(IIdentityService identityService,
        ISetRepository setRepository, ILogger<DeleteSetCommandHandler> logger) {
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _setRepository = setRepository ??
            throw new ArgumentNullException(nameof(setRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult> Handle(DeleteSetCommand command,
        CancellationToken cancellationToken) {
        (await _setRepository.GetAsync(command.Id,
            _identityService.GetUserIdentityGuid())).SetDeleted();
        return await _setRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}