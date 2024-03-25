using Bookery.Node.Common.DTOs.Output;
using Bookery.Node.Data.Entities;

namespace Bookery.Node.Mappers;

public class NodeMapper
{
    public static NodeDto ToDto(NodeEntity entity) =>
        new(Id: entity.Id,
            Name: entity.Name,
            IsDirectory: entity.IsDirectory,
            Size: entity.Size,
            ParentId: entity.ParentId,
            OwnerId: entity.OwnerId,
            CreationTimestamp: entity.CreatedAt,
            CreatedById: entity.CreatedById,
            ModificationTimestamp: entity.ModifiedAt,
            ModifiedById: entity.ModifiedById
        );
}