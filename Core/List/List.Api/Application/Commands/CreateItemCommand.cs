using System.Text.Json.Nodes;
using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

public class CreateItemCommand : IRequest<ServiceResult> {
    public int SetId { get; set; }

    public JsonObject CreateContribJson { get; set; }

    public CreateItemCommand(int setId, JsonObject createContribJson) {
        SetId = setId;
        CreateContribJson = createContribJson;
    }
}