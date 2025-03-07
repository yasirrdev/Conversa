namespace Conversa.Models.Databases.Dtos;

public class ContactDto
{
    public Guid ContactId { get; set; }
    public string Nickname { get; set; }
    public DateTime DateAdded { get; set; }
}
