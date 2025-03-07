namespace Conversa.Models.Databases.Entities;

public class Contacts
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ContactId { get; set; } 
    public string? Nickname { get; set; } 
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    public Users User { get; set; }
    public Users ContactUser { get; set; }
}

