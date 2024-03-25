namespace Bookery.Authentication.Data;

public class AppConfiguration
{
    public AuthenticationConfiguration Authentication { get; }

    public AppConfiguration(IConfiguration configuration)
    {
        Authentication = new(configuration);
    }
    
    public class AuthenticationConfiguration
    {
        public string Issuer { get; }
        public string Audience { get; }
        public int AccessTokenExpirationInSeconds { get; }
        public int RefreshTokenExpirationInSeconds { get; }
        public string SigningKey { get; }

        public AuthenticationConfiguration(IConfiguration configuration)
        {
            Issuer = configuration["Authentication:Issuer"];
            Audience = configuration["Authentication:Audience"];
            AccessTokenExpirationInSeconds =
                int.TryParse(configuration["Authentication:AccessTokenExpirationInSeconds"], out var parsedAccessTokenExpirationInSeconds)
                    ? parsedAccessTokenExpirationInSeconds
                    : 600;
            RefreshTokenExpirationInSeconds =
                int.TryParse(configuration["Authentication:RefreshTokenExpirationInSeconds"], out var parsedRefreshTokenExpirationInSeconds)
                    ? parsedRefreshTokenExpirationInSeconds
                    : 6000;
            SigningKey = configuration["Authentication:SigningKey"];
        }
    }
}