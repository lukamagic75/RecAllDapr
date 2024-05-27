using System.Runtime.InteropServices.JavaScript;
using Dapper;
using Microsoft.Data.SqlClient;

namespace RecAll.Core.List.Api.Application.Queries;

public class ListQueryService : IListQueryService {
    private readonly string _connectionString;

    public ListQueryService(IConfiguration configuration) {
        _connectionString = configuration["ConnectionStrings:ListContext"];
    }

    public async Task<(IEnumerable<ListViewModel>, int)> ListAsync(int skip,
        int take, string userIdentityGuid) {
        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<dynamic>(
            @"select Id, Name, TypeId, Total=count(*) over () 
                    from lists
                    where UserIdentityGuid=@userIdentityGuid 
                        and IsDeleted='false'
                    order by Id desc
                    offset @skip rows
                    fetch next @take rows only;",
            new { userIdentityGuid, skip, take });

        return result.Any()
            ? (
                result.Select(p => new ListViewModel {
                    Id = p.Id, Name = p.Name, TypeId = p.TypeId
                }), (int)result.First().Total)
            : (Enumerable.Empty<ListViewModel>(), 0);
    }

    public async Task<(IEnumerable<ListViewModel>, int)> ListAsync(int typeId,
        int skip, int take, string userIdentityGuid) {
        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<dynamic>(
            @"select Id, Name, TypeId, Total=count(*) over () 
                    from lists
                    where TypeId=@typeId 
                      and UserIdentityGuid=@userIdentityGuid  
                      and IsDeleted='false'
                    order by Id desc
                    offset @skip rows
                    fetch next @take rows only;",
            new { typeId, userIdentityGuid, skip, take });

        return result.Any()
            ? (
                result.Select(p => new ListViewModel {
                    Id = p.Id, Name = p.Name, TypeId = p.TypeId
                }), (int)result.First().Total)
            : (Enumerable.Empty<ListViewModel>(), 0);
    }

    public async Task<ListViewModel> GetAsync(int id, string userIdentityGuid) {
        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<dynamic>(
            @"select Id, Name, TypeId
                    from lists
                    where Id=@id
                        and UserIdentityGuid=@userIdentityGuid
                        and IsDeleted='false'", new { id, userIdentityGuid });

        if (!result.Any()) {
            return null;
        }

        var p = result.First();
        return new ListViewModel {
            Id = p.Id, Name = p.Name, TypeId = p.TypeId
        };
    }
}