using Bookery.Authentication.Models;
using Bookery.Services.Table;

namespace Bookery.Authentication.Repositories.User;

public class StsUserRepository : IStsUserRepository
{
    private readonly ITableService<StsUser> _tableService;

    public StsUserRepository(string connectionString)
    {
        _tableService = new TableService<StsUser>(connectionString, "users");
    }

    public async Task<StsUser> GetByEmail(string email)
    {
        var userEntity = await _tableService.RetrieveAsync("users", email);

        if (userEntity == null)
        {
            return null;
        }

        return userEntity;
    }
}