using Bookery.Authentication.Models;
using Bookery.Authentication.Repositories.User;

namespace Bookery.Authentication.Services.Common;

public class HeaderService : IHeaderService
{
    private readonly IStsUserRepository _stsUserRepository;

    public HeaderService(IStsUserRepository stsUserRepository)
    {
        _stsUserRepository = stsUserRepository;
    }

    public async Task<StsUser?> GetRequestUser(HttpRequest request)
    {
        StsUser? user = null;
        if (request.Headers.TryGetValue("Email", out var email))
        {
            user = await _stsUserRepository.GetByEmail(email);
        }

        return user;
    }
}