using MediatR;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class DeleteListCommandHandler : IRequestHandler<DeleteListCommand,
    ServiceResult> {
    private readonly IIdentityService _identityService;
    private readonly IListRepository _listRepository;
    private readonly ILogger<DeleteListCommandHandler> _logger;

    public DeleteListCommandHandler(IIdentityService identityService,
        IListRepository listRepository,
        ILogger<DeleteListCommandHandler> logger) {
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _listRepository = listRepository ??
            throw new ArgumentNullException(nameof(listRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult> Handle(DeleteListCommand command,
        CancellationToken cancellationToken) {
        (await _listRepository.GetAsync(command.Id,
            _identityService.GetUserIdentityGuid())).SetDeleted();
        return await _listRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}