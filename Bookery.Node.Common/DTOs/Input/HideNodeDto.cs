namespace Bookery.Node.Common.DTOs.Input;

public record HideNodeDto(
    Guid NodeId,
    Guid UserId
    );