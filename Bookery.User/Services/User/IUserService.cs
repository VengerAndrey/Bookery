using Bookery.Services.Common;

namespace Bookery.User.Services.User;

public interface IUserService : IDataService<Models.User>
{
    Task<Models.User> GetByEmail(string email);
}