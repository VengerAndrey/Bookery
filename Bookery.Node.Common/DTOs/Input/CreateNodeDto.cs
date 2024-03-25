namespace Bookery.Node.Common.DTOs.Input;

public record CreateNodeDto(
    string Name,
    bool IsDirectory
    );