namespace Bookery.Authentication.Common.DTOs.Input;

public record UserSignUpDto(
    Guid Id,
    string Email,
    string Password
    );