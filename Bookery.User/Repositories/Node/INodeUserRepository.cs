using Bookery.User.Entities;

namespace Bookery.User.Repositories.Node;

public interface INodeUserRepository
{
    Task<bool> Add(NodeUser nodeUser);
}