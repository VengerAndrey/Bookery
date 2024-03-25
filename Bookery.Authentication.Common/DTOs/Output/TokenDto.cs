namespace Bookery.Authentication.Common.DTOs.Output;

public record TokenDto(
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpireAt
);