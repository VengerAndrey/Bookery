namespace Bookery.Node.Common.DTOs.Input;

public record UpdateNodeDto(
    string? Name,
    long? Size,
    Guid ModifiedById
    );