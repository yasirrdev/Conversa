using Conversa.Models.Databases;
using Conversa.Models.Databases.Entities;
using Microsoft.EntityFrameworkCore;

namespace Conversa.Repositories;

public class MessagesRepository
{
    private readonly DataContext _context;

    public MessagesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Messages>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task SaveMessageAsync(Messages message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }
}
