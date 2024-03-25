namespace Bookery.Node.Common.DTOs.Output;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName
    );