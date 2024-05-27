using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateSetCommand : IRequest<ServiceResult> {
    public string Name { get; set; }

    public int ListId { get; set; }

    public CreateSetCommand(string name, int listId) {
        Name = name;
        ListId = listId;
    }
}