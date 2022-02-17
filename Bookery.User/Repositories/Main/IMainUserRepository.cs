using Bookery.Services.Common;

namespace Bookery.User.Repositories.Main;

public interface IMainUserRepository : IDataService<Models.User>
{
    Task<Models.User> GetByEmail(string email);
}