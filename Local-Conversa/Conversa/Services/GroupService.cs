using Conversa.Models.Databases.Dtos;
using Conversa.Models.Databases.Entities;
using Conversa.Models.Interfaces;

namespace Conversa.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GroupService(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GroupDto> CreateGroupWithContactsAsync(Guid creatorId, string groupName, string? description, List<Guid> contactIds)
    {
        var creator = await _userRepository.GetUserWithContactsAsync(creatorId);
        if (creator == null)
            throw new Exception("User not found");

        var validContactIds = creator.Contacts
            .Select(c => c.ContactId)
            .ToList();

        var invalidContacts = contactIds
            .Where(id => !validContactIds.Contains(id))
            .ToList();

        if (invalidContacts.Any())
            throw new Exception($"The following contact IDs are not in your contacts: {string.Join(", ", invalidContacts)}");

        var group = new Groups
        {
            Id = Guid.NewGuid(),
            Name = groupName,
            Description = description,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow,
            Creator = creator
        };

        var creatorGroup = new Users_Groups
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = creatorId,
            Role = "admin",
            JoinedAt = DateTime.UtcNow,
            Group = group,
            User = creator
        };

        group.UserGroups.Add(creatorGroup);

        foreach (var contactId in contactIds)
        {
            var userGroup = new Users_Groups
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                UserId = contactId,
                Role = "member",
                JoinedAt = DateTime.UtcNow,
                Group = group
            };

            group.UserGroups.Add(userGroup);
        }

        await _groupRepository.CreateGroupAsync(group);

        var memberIds = group.UserGroups.Select(ug => ug.UserId).ToList();

        var members = new List<MemberDto>();
        foreach (var memberId in memberIds)
        {
            var user = await _userRepository.GetUserByIdAsync(memberId); 
            if (user != null)
            {
                members.Add(new MemberDto
                {
                    Id = user.Id,
                    Name = user.Name
                });
            }
        }

        // Mapear a DTO
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatorId = group.CreatorId,
            CreatedAt = group.CreatedAt,
            CreatorName = creator.Name,
            Members = members
        };
    }
}