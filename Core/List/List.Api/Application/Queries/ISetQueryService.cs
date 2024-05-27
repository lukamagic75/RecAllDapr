namespace RecAll.Core.List.Api.Application.Queries;

public interface ISetQueryService {
    Task<(IEnumerable<SetViewModel>, int)> ListAsync(int listId, int skip,
        int take, string userIdentityGuid);

    Task<SetViewModel> GetAsync(int id, string userIdentityGuid);
}