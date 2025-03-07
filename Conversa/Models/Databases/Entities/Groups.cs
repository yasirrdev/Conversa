namespace Conversa.Models.Databases.Entities;

public class Groups
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Users Creator { get; set; }
    public ICollection<Users_Groups> UserGroups { get; set; } = new List<Users_Groups>();
    public ICollection<Messages> Messages { get; set; } = new List<Messages>();
}

