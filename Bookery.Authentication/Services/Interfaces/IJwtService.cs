using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Authentication.Common.DTOs.Output;
using Bookery.Authentication.Data.ValueObjects;

namespace Bookery.Authentication.Services.Interfaces;

public interface IJwtService
{
    TokenDto Authenticate(UserClaimsValueObject userClaims);
    TokenDto? Refresh(UserClaimsValueObject userClaims, RefreshTokenDto refreshTokenDto);
    void ClearExpiredRefreshTokens(DateTime now);
    void ClearRefreshToken(string email);
}