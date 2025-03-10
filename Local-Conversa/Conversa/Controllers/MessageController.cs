using Conversa.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly MessagesRepository _messagesRepository;

    public MessagesController(MessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    [HttpGet("chat/{userId1}/{userId2}")]
    public async Task<IActionResult> GetChatMessages(Guid userId1, Guid userId2)
    {
        var messages = await _messagesRepository.GetMessagesBetweenUsersAsync(userId1, userId2);

        if (!messages.Any())
        {
            return NotFound(new { message = "No hay mensajes entre estos usuarios." });
        }

        return Ok(messages);
    }
}

