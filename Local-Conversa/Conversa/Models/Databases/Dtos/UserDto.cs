namespace Conversa.Models.Databases.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Status { get; set; }
}

public class UserCreateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
}
