using System.ComponentModel.DataAnnotations;

namespace Conversa.Models.Databases.Entities;

public class Users
{
    [Key]
    public Guid Id { get; set; } 
    public string Name { get; set; }
    [Phone]
    public string Phone { get; set; } 
    public string Password { get; set; }
    public string Status { get; set; } = "Hi, I'm using Conversa!!!";
    //public string? ProfilePicture { get; set; } 

    public ICollection<Contacts> Contacts { get; set; }
    public ICollection<Users_Groups> UserGroups { get; set; }
    public ICollection<Messages> Messages { get; set; }
}

