namespace Bookery.Authentication.Data.ValueObjects;

public record UserClaimsValueObject(
    Guid Id,
    string Email
);