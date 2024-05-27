using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class DeleteSetCommand : IRequest<ServiceResult> {
    public int Id { get; set; }

    public DeleteSetCommand(int id) {
        Id = id;
    }
}