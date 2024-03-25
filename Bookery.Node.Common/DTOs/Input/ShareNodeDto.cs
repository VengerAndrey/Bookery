using Bookery.Node.Common.Enums;

namespace Bookery.Node.Common.DTOs.Input;

public record ShareNodeDto(
    Guid NodeId,
    Guid UserId,
    AccessTypeId AccessTypeId
    );