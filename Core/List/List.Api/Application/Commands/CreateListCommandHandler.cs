using MediatR;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateListCommandHandler : IRequestHandler
    <CreateListCommand, ServiceResult> {
    private readonly IIdentityService _identityService;
    private readonly IListRepository _listRepository;

    public CreateListCommandHandler(IIdentityService identityService,
        IListRepository listRepository) {
        _identityService = identityService;
        _listRepository = listRepository;
    }

    public async Task<ServiceResult> Handle(CreateListCommand command,
        CancellationToken cancellationToken) {
        _listRepository.Add(new Domain.AggregateModels.ListAggregate.List(
            command.Name, command.TypeId,
            _identityService.GetUserIdentityGuid()));
        return await _listRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}