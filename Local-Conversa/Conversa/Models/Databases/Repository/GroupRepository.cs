using Conversa.Models.Databases.Entities;
using Conversa.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Conversa.Models.Databases.Repository;

public class GroupRepository : IGroupRepository
{
    private readonly DataContext _context;

    public GroupRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Groups> CreateGroupAsync(Groups group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task AddUserToGroupAsync(Users_Groups userGroup)
    {
        _context.Users_Groups.Add(userGroup);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> GroupExistsAsync(Guid groupId)
    {
        return await _context.Groups.AnyAsync(g => g.Id == groupId);
    }
}