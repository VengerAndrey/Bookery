using Bookery.User.Common.DTOs.Output;
using Bookery.User.Data.Entities;

namespace Bookery.User.Mappers;

public class UserMapper
{
    public static UserDto ToDto(UserEntity entity) =>
        new(Id: entity.Id,
            Email: entity.Email,
            FirstName: entity.FirstName,
            LastName: entity.LastName);
}