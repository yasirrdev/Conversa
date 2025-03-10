namespace Conversa.Models.Databases.Dtos;

public class CreateGroupDto
{
    public Guid CreatorId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<Guid> ContactIds { get; set; }
}
