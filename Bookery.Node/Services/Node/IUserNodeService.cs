using Bookery.Node.Models;

namespace Bookery.Node.Services.Node;

public interface IUserNodeService
{
    Task<IEnumerable<UserNode>> GetAll();
    Task<UserNode> Create(UserNode entity);
    Task<UserNode> Update(UserNode entity);
    Task<bool> Delete(UserNode entity);
}