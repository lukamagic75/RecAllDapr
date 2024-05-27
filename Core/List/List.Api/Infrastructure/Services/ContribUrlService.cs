using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Api.Infrastructure.Services;

public class ContribUrlService : IContribUrlService {
    public string GetContribUrl(int listTypeId) {
        string route;

        if (listTypeId == ListType.Text.Id) {
            route = "text";  //根据不同的类型转发到不同的网关
        } 
        else if (listTypeId == ListType.MaskedText.Id)
        {
            route = "maskedtext";

        }else {
            throw new ArgumentOutOfRangeException(nameof(listTypeId),
                $"有效取值为{string.Join(",", Enumeration.GetAll<ListType>().Select(p => p.Id.ToString()))}");
        }

        return $"http://recall-envoygateway/{route}";
    }
}