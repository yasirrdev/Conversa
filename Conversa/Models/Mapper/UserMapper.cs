namespace Conversa.Models.Mapper;

using Conversa.Models.Databases.Dtos;
using Conversa.Models.Databases.Entities;

public class UserMapper
{

    public UserDto ToDto(Users user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Status = user.Status,
        };
    }

    public IEnumerable<UserDto> usersToDto(IEnumerable<Users> users)
    {
        return users.Select(ToDto);
    }

}
