using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateListCommand : IRequest<ServiceResult> {
    public string Name { get; set; }

    public int TypeId { get; set; }

    public CreateListCommand(string name, int typeId) {
        Name = name;
        TypeId = typeId;
    }
}