namespace Bookery.User.Common.DTOs.Input;

public record UserSignUpDto(
    string Email,
    string Password,
    string FirstName,
    string LastName
    );