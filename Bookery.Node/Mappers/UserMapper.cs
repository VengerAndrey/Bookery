using Bookery.Node.Common.DTOs.Output;
using Bookery.Node.Data.Entities;

namespace Bookery.Node.Mappers;

public class UserMapper
{
    public static UserDto ToDto(UserEntity entity) =>
        new(Id: entity.Id,
            Email: entity.Email,
            FirstName: entity.FirstName,
            LastName: entity.LastName);
}