using Bookery.Services.Table;
using Bookery.User.Entities;

namespace Bookery.User.Repositories.STS;

public class StsUserRepository : IStsUserRepository
{
    private readonly ITableService<StsUser> _table;

    public StsUserRepository(string connectionString)
    {
        _table = new TableService<StsUser>(connectionString, "users");
    }

    public async Task<bool> Add(StsUser stsUser)
    {
        var result = await _table.InsertOrMergeAsync(stsUser);

        return result != null;
    }
}