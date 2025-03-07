using System.Text.RegularExpressions;

namespace Conversa.Models.Databases.Entities;

public class Messages
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public string MessageType { get; set; } = "text"; 
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "sent"; 

    public Groups Group { get; set; }
    public Users Sender { get; set; }
}
