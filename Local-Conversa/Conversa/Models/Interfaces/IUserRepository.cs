using Conversa.Models.Databases.Dtos;
using Conversa.Models.Databases.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Conversa.Models.Interfaces;

public interface IUserRepository
{
    Task<ICollection<UserDto>> GetUsersAsync();
    Task CreateUserAsync(UserCreateDto user);
    Task<Users> GetUserByPhoneAsync(string phoneNumber);
    Task<Users> GetUserWithContactsAsync(Guid userId);
    Task<Users> GetUserByIdAsync(Guid userId);

}
