namespace Conversa.Models.Databases.Entities;

public class Users_Groups
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "member";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Groups Group { get; set; } 
    public Users User { get; set; }   
}
