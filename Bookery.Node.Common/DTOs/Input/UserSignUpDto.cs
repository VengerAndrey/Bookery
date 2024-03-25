namespace Bookery.Node.Common.DTOs.Input;

public record UserSignUpDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName
    );