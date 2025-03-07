using Conversa.Models.Databases.Dtos;

namespace Conversa.Models.Interfaces;

public interface IContactRepository
{
    Task<bool> AddContactByPhoneAsync(string userPhone, string contactPhone, string nickname);
    Task<List<ContactDto>> GetContactsByUserIdAsync(Guid userId);
}
