namespace Bookery.Node.Common.DTOs.Output;

public record NodeDto(
    Guid Id,
    string? Name,
    bool IsDirectory,
    long? Size,
    Guid? ParentId,
    Guid OwnerId,
    long CreationTimestamp,
    Guid CreatedById,
    long ModificationTimestamp,
    Guid ModifiedById
);