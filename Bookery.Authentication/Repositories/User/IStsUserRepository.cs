using Bookery.Authentication.Models;

namespace Bookery.Authentication.Repositories.User;

public interface IStsUserRepository
{
    Task<StsUser> GetByEmail(string email);
}