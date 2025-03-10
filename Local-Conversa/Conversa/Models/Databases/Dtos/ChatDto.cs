namespace Conversa.Models.Databases.Dtos;

public class ChatDTO
{
    public bool gameChatMessage { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public string Message { get; set; }
    public bool IsSender { get; set; }
}
