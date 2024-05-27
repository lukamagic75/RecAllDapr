using Dapper;
using Microsoft.Data.SqlClient;

namespace RecAll.Core.List.Api.Application.Queries;

public class SetQueryService : ISetQueryService {
    private readonly string _connectionString;

    public SetQueryService(IConfiguration configuration) {
        _connectionString = configuration["ConnectionStrings:ListContext"];
    }


    public async Task<(IEnumerable<SetViewModel>, int)> ListAsync(int listId,
        int skip, int take, string userIdentityGuid) {
        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<dynamic>(@"select s.Id as Id,
                        s.Name as Name,
                        s.TypeId as TypeId,
                        s.ListId as ListId,
                        Total=count(*) over ()
                    from sets as s
                    where s.ListId=@listId
                        and s.UserIdentityGuid=@userIdentityGuid
                        and s.IsDeleted='false'
                    order by Id
                    offset @skip rows
                    fetch next @take rows only;",
            new { listId, userIdentityGuid, skip, take });

        return result.Any()
            ? (
                result.Select(p => new SetViewModel {
                    Id = p.Id,
                    Name = p.Name,
                    TypeId = p.TypeId,
                    ListId = p.ListId
                }), (int)result.First().Total)
            : (Enumerable.Empty<SetViewModel>(), 0);
    }

    public async Task<SetViewModel> GetAsync(int id, string userIdentityGuid) {
        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<dynamic>(
            @"select Id, Name, TypeId, ListId
                    from sets
                    where Id=@id
                        and UserIdentityGuid=@userIdentityGuid
                        and IsDeleted='false'", new { id, userIdentityGuid });

        if (!result.Any()) {
            return null;
        }

        var p = result.First();
        return new SetViewModel {
            Id = p.Id, Name = p.Name, TypeId = p.TypeId, ListId = p.ListId
        };
    }
}