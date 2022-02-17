using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bookery.Config;

public class AuthConfig
{
    public const string Issuer = "Bookery.STS";
    public const string Audience = "Bookery.Client";
    public const int AccessTokenExpiration = 600;
    public const int RefreshTokenExpiration = 3000;

    // must be stored in a secure place
    private const string Key = "ZOVa6jnmVUi69qyaT8jPjv9cUVwCbTXeQtOOTkbfCgn9QSbWTxeFX53tz8ougBPA";

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.Unicode.GetBytes(Key));
    }

    public static SymmetricSecurityKey GetSymmetricSecurityKey1()
    {
        return new SymmetricSecurityKey(Encoding.Unicode.GetBytes(Key + "1"));
    }
}