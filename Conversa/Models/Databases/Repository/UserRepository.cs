using Conversa.Models.Databases.Dtos;
using Conversa.Models.Databases.Entities;
using Conversa.Models.Interfaces;
using Conversa.Models.Mapper;
using Conversa.Services;
using Microsoft.EntityFrameworkCore;

namespace Conversa.Models.Databases.Repository;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly UserMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(DataContext context, UserMapper mapper, IPasswordHasher passwordHasher)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }
    public async Task<ICollection<UserDto>> GetUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Phone = u.Phone,
                Status = u.Status
            })
            .ToListAsync();
    }


    public async Task CreateUserAsync(UserCreateDto userCreateDto)
    {
        var user = new Users
        {
            Id = userCreateDto.Id,
            Name = userCreateDto.Name,
            Phone = userCreateDto.Phone,
            Password = userCreateDto.Password,
        };

        var passwordHasher = new PasswordService();
        user.Password = passwordHasher.Hash(userCreateDto.Password);

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();
    }

    public async Task<Users> GetUserByPhoneAsync(string phone)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Phone.Equals(phone));
    }
}
