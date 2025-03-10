namespace Conversa.Models.Databases.Dtos;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CreatorName { get; set; }

    public List<MemberDto> Members { get; set; } = new List<MemberDto>();
}

public class MemberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
