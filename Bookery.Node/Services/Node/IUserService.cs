using Bookery.Node.Models;
using Bookery.Services.Common;

namespace Bookery.Node.Services.Node;

public interface IUserService : IDataService<User>
{
    Task<User> GetByEmail(string email);
}