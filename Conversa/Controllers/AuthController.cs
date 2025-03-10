using Conversa.Models.Databases;
using Conversa.Models.Databases.Dtos;
using Conversa.Models.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using LoginRequest = Conversa.Models.Databases.Dtos.LoginRequest;
namespace Conversa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenValidationParameters _tokenParameters;
    private readonly DataContext _context;
    private readonly IPasswordHasher _passwordHash;

    public AuthController(IOptionsMonitor<JwtBearerOptions> jwOptions, DataContext context, IPasswordHasher passwordHash)
    {
        _tokenParameters = jwOptions.Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;
        _context = context;
        _passwordHash = passwordHash;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest model)
    {
        string hashedPassword = _passwordHash.Hash(model.Password);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => (u.Phone.Equals(model.Identification) || u.Name.Equals(model.Identification)) && u.Password == hashedPassword);

        if (user != null)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
            {
                { "Id", user.Id.ToString()},
                { "Name", user.Name },
                { "Phone", user.Phone },
                { "Status", user.Status }
            },
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    _tokenParameters.IssuerSigningKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string stringToken = tokenHandler.WriteToken(token);

            return Ok(new LoginResult { AccessToken = stringToken });
        }
        else
        {
            return Unauthorized(new { message = "Contraseña o Datos de usuario Incorrectos" });
        }
    }
}
