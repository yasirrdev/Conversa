using Conversa.Models.Databases;
using Conversa.Models.Databases.Dtos;
using Conversa.Models.Interfaces;
using Conversa.Models.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conversa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserRepository _userIRepository;
    private readonly UserMapper _userMapper;
    private readonly DataContext _dataContext;

    public UserController(IUserRepository userIRepository, UserMapper userMapper, DataContext dataContext)
    {
        _userIRepository = userIRepository;
        _userMapper = userMapper;
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var users = await _userIRepository.GetUsersAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> AddUserAsync([FromForm] UserCreateDto userToAddDto)
    {
        if (userToAddDto == null)
        {
            return BadRequest(new
            {
                message = "Información necesaria no enviada.",
                code = "MISSING_REQUIRED_INFORMATION"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingPhone = await _userIRepository.GetUserByPhoneAsync(userToAddDto.Phone);
        if (existingPhone != null)
        {
            return Conflict(new
            {
                message = "Télefono existente, por favor introduzca otro.",
                code = "PHONE_ALREADY_EXISTS"
            });
        }

        try
        {
            await _userIRepository.CreateUserAsync(userToAddDto);
            return Ok(new { message = "Usuario registrado con éxito" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}. Inner Exception: {ex.InnerException?.Message}");
        }

    }
}