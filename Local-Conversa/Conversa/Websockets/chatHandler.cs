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
    public async Task<bool> SendGroupMessageAsync(Guid senderId, Guid groupId, string content)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var _webSocketHandler = scope.ServiceProvider.GetRequiredService<websocketHandler>();

        try
        {
            Console.WriteLine($"Iniciando SendGroupMessageAsync - SenderId: {senderId}, GroupId: {groupId}");

            // Validar SenderId
            var senderExists = await _context.Users.AnyAsync(u => u.Id == senderId);
            if (!senderExists)
            {
                Console.WriteLine($"❌ SenderId {senderId} no encontrado en Users.");
                throw new Exception("Sender not found");
            }

            // Validar GroupId
            var group = await _context.Groups
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                Console.WriteLine($"❌ Grupo {groupId} no encontrado.");
                throw new Exception("Group not found");
            }

            var message = new Messages
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                GroupId = groupId,
                ReceiverId = null,
                Content = content,
                MessageType = "text",
                SentAt = DateTime.UtcNow,
                Status = "sent"
            };

            Console.WriteLine($"Guardando mensaje {message.Id} con Status: {message.Status}");
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Mensaje {message.Id} guardado correctamente.");

            var memberIds = group.UserGroups.Select(ug => ug.UserId).ToList();
            bool atLeastOneDelivered = false;

            foreach (var memberId in memberIds)
            {
                if (memberId == senderId) continue;

                Console.WriteLine($"Enviando mensaje a {memberId}");
                bool isDelivered = await _webSocketHandler.SendMessageToUser(memberId.ToString(), JsonSerializer.Serialize(new
                {
                    action = "new_group_message",
                    messageId = message.Id,
                    groupId,
                    senderId,
                    content,
                    sentAt = message.SentAt,
                    status = "unread"
                }));

                if (isDelivered)
                {
                    atLeastOneDelivered = true;
                    Console.WriteLine($"Mensaje entregado a {memberId}");
                }
            }

            message.Status = atLeastOneDelivered ? "delivered" : "not_received";
            Console.WriteLine($"Actualizando estado del mensaje {message.Id} a {message.Status}");
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Estado del mensaje {message.Id} actualizado correctamente.");

            await _webSocketHandler.SendMessageToUser(senderId.ToString(), JsonSerializer.Serialize(new
            {
                action = "message_status",
                messageId = message.Id,
                status = message.Status
            }));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en SendGroupMessageAsync: {ex.Message}");
            Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
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

    public async Task<List<Messages>> GetGroupChatHistoryAsync(Guid groupId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<DataContext>();

        return await _context.Messages
            .Where(m => m.GroupId == groupId)
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
            .Where(m => m.ReceiverId == receiverId && m.Status == "not_received" && m.GroupId == null)
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

        var pendingGroupMessages = await _context.Messages
            .Include(m => m.Group)
            .ThenInclude(g => g.UserGroups)
            .Where(m => m.GroupId != null && m.Status == "not_received" &&
                       m.Group.UserGroups.Any(ug => ug.UserId == receiverId))
            .ToListAsync();

        foreach (var message in pendingGroupMessages)
        {
            bool isDelivered = await _webSocketHandler.SendMessageToUser(receiverId.ToString(), JsonSerializer.Serialize(new
            {
                action = "new_group_message",
                messageId = message.Id,
                groupId = message.GroupId,
                senderId = message.SenderId,
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
