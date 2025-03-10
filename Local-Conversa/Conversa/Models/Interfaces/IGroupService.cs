using Conversa.Models.Databases.Dtos;
using Conversa.Models.Databases.Entities;

namespace Conversa.Models.Interfaces;

public interface IGroupService
{
    Task<GroupDto> CreateGroupWithContactsAsync(Guid creatorId, string groupName, string? description, List<Guid> contactIds);
}
