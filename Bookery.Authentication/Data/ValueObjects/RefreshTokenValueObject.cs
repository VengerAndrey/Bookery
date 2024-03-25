namespace Bookery.Authentication.Data.ValueObjects;

public record RefreshTokenValueObject(
    string Email,
    string Token,
    DateTime ExpireAt
    );