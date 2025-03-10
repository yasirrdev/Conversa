using Conversa.Models.Databases.Entities;

namespace Conversa.Models.Interfaces;

public interface IGroupRepository
{
    Task<Groups> CreateGroupAsync(Groups group);
    Task AddUserToGroupAsync(Users_Groups userGroup);
    Task<bool> GroupExistsAsync(Guid groupId);
}