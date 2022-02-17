using Bookery.User.Entities;

namespace Bookery.User.Repositories.STS;

public interface IStsUserRepository
{
    Task<bool> Add(StsUser stsUser);
}