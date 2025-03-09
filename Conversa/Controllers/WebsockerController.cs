using Conversa.Websockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Conversa.Controllers;

public class WebSocketController : ControllerBase
{
    private readonly websocketHandler _websocketHandler;

    public WebSocketController(websocketHandler websocketHandler)
    {
        _websocketHandler = websocketHandler;
    }

    [HttpGet("ws")]
    public async Task ConnectHandler()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsync("Se esperaba una solicitud WebSocket");
            return;
        }

        string token = HttpContext.Request.Query["token"].ToString();
        if (string.IsNullOrEmpty(token) && HttpContext.Request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }
        }

        if (string.IsNullOrEmpty(token))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsync("Token JWT requerido");
            return;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrEmpty(key))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await HttpContext.Response.WriteAsync("JWT_KEY no configurado");
                return;
            }

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await HttpContext.Response.WriteAsync("UserId no encontrado en el token");
                return;
            }

            HttpContext.Items["userId"] = userId;

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _websocketHandler.HandleAsync(HttpContext, webSocket);
        }
        catch (SecurityTokenException ex)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsync($"Token inválido: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en conexión WebSocket: {ex.Message}");
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await HttpContext.Response.WriteAsync("Error estableciendo conexión WebSocket");
        }
    }
}