using Conversa.Models.Databases.Entities;
using Conversa.Models.Databases;
using Conversa.Websockets;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class ChatHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ChatHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<bool> SendMessageAsync(Guid senderId, Guid receiverId, string content)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var _webSocketHandler = scope.ServiceProvider.GetRequiredService<websocketHandler>();

        var message = new Messages
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            MessageType = "text",
            SentAt = DateTime.UtcNow,
            Status = "sent"
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        bool isDelivered = await _webSocketHandler.SendMessageToUser(receiverId.ToString(), JsonSerializer.Serialize(new
        {
            action = "new_message",
            messageId = message.Id,
            senderId,
            receiverId,
            content,
            sentAt = message.SentAt,
            status = "unread"
        }));

        if (isDelivered)
        {
            message.Status = "delivered";
            await _context.SaveChangesAsync();

            await _webSocketHandler.SendMessageToUser(senderId.ToString(), JsonSerializer.Serialize(new
            {
                action = "message_status",
                messageId = message.Id,
                status = "delivered"
            }));
        }
        else
        {
            message.Status = "not_received";
            await _context.SaveChangesAsync();

            await _webSocketHandler.SendMessageToUser(senderId.ToString(), JsonSerializer.Serialize(new
            {
                action = "message_status",
                messageId = message.Id,
                status = "not_received"
            }));
        }

        return true;
    }

    public async Task<List<Messages>> GetChatHistoryAsync(Guid userId, Guid contactId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();

        return await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == contactId) ||
                        (m.SenderId == contactId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task MarkMessagesAsReadAsync(Guid userId, Guid contactId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var _webSocketHandler = scope.ServiceProvider.GetRequiredService<websocketHandler>();

        var unreadMessages = await _context.Messages
            .Where(m => m.SenderId == contactId && m.ReceiverId == userId && m.Status != "read")
            .ToListAsync();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.Status = "read";
                _context.Messages.Update(message);

                await _webSocketHandler.SendMessageToUser(contactId.ToString(), JsonSerializer.Serialize(new
                {
                    action = "message_status",
                    messageId = message.Id,
                    status = "read"
                }));
            }
            await _context.SaveChangesAsync();
        }
    }
    public async Task SendPendingMessagesAsync(string userId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var _webSocketHandler = scope.ServiceProvider.GetRequiredService<websocketHandler>();

        if (!Guid.TryParse(userId, out Guid receiverId))
        {
            return;
        }

        var pendingMessages = await _context.Messages
            .Where(m => m.ReceiverId == receiverId && m.Status == "not_received")
            .ToListAsync();

        foreach (var message in pendingMessages)
        {
            bool isDelivered = await _webSocketHandler.SendMessageToUser(receiverId.ToString(), JsonSerializer.Serialize(new
            {
                action = "new_message",
                messageId = message.Id,
                senderId = message.SenderId,
                receiverId = message.ReceiverId,
                content = message.Content,
                sentAt = message.SentAt,
                status = "unread"
            }));

            if (isDelivered)
            {
                message.Status = "delivered";
                await _context.SaveChangesAsync();

                await _webSocketHandler.SendMessageToUser(message.SenderId.ToString(), JsonSerializer.Serialize(new
                {
                    action = "message_status",
                    messageId = message.Id,
                    status = "delivered"
                }));
            }
        }
    }
}
