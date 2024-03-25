namespace Bookery.Authentication.Common.DTOs.Input;

public record GetTokenDto(
    string Email,
    string Password
    );