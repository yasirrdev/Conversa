using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

public class websocketHandler
{
    private static readonly ConcurrentDictionary<string, WebSocket> _connections = new();
    //private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public async Task HandleAsync(HttpContext context, WebSocket webSocket)
    {
        var userId = context.Items["userId"]?.ToString();
        if (string.IsNullOrEmpty(userId))
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Falta el userId", CancellationToken.None);
            return;
        }

        //await _semaphore.WaitAsync();
        try
        {
            _connections[userId] = webSocket;
            Console.WriteLine($"🔗 Usuario {userId} conectado");
            var chatHandlerPending = context.RequestServices.GetRequiredService<ChatHandler>();
            await chatHandlerPending.SendPendingMessagesAsync(userId);

            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cierre normal", CancellationToken.None);
                    break;
                }

                if (result.MessageType != WebSocketMessageType.Text)
                    continue;

                try
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var request = JsonSerializer.Deserialize<Dictionary<string, string>>(message);

                    if (request != null && request.TryGetValue("action", out string action))
                    {
                        switch (action)
                        {
                            case "send_message":
                                if (request.TryGetValue("receiverId", out string receiverIdStr) &&
                                    request.TryGetValue("content", out string content) &&
                                    Guid.TryParse(userId, out Guid senderId) &&
                                    Guid.TryParse(receiverIdStr, out Guid receiverId))
                                {
                                    var chatHandler = context.RequestServices.GetRequiredService<ChatHandler>();
                                    await chatHandler.SendMessageAsync(senderId, receiverId, content);
                                }
                                break;

                            case "send_group_message":
                                if (request.TryGetValue("groupId", out string groupIdStr) &&
                                    request.TryGetValue("content", out string contentGroup) &&
                                    Guid.TryParse(userId, out Guid senderIdGroup) &&
                                    Guid.TryParse(groupIdStr, out Guid groupId))
                                {
                                    var chatHandler = context.RequestServices.GetRequiredService<ChatHandler>();
                                    await chatHandler.SendGroupMessageAsync(senderIdGroup, groupId, contentGroup);
                                }
                                else
                                {
                                    var errorResponseGroup = new
                                    {
                                        success = false,
                                        message = "❌ Parámetros inválidos para send_group_message."
                                    };
                                    await SendMessageToUser(userId, JsonSerializer.Serialize(errorResponseGroup));
                                }
                                break;

                            case "mark_as_read":
                                if (request.TryGetValue("contactId", out string contactIdStr) &&
                                    Guid.TryParse(userId, out Guid currentUserId) &&
                                    Guid.TryParse(contactIdStr, out Guid contactId))
                                {
                                    var chatHandler = context.RequestServices.GetRequiredService<ChatHandler>();
                                    await chatHandler.MarkMessagesAsReadAsync(currentUserId, contactId);
                                }
                                break;

                            default:
                                var errorResponse = new
                                {
                                    success = false,
                                    message = "❌ Acción no reconocida."
                                };
                                await SendMessageToUser(userId, JsonSerializer.Serialize(errorResponse));
                                break;
                        }
                    }
                    else
                    {
                        var errorResponse = new
                        {
                            success = false,
                            message = "❌ Mensaje inválido o acción no especificada."
                        };
                        await SendMessageToUser(userId, JsonSerializer.Serialize(errorResponse));
                    }
                }
                catch (JsonException)
                {
                    var errorResponse = new
                    {
                        success = false,
                        message = "❌ Formato JSON inválido."
                    };
                    await SendMessageToUser(userId, JsonSerializer.Serialize(errorResponse));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en WebSocket para usuario {userId}: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
        finally
        {
            _connections.TryRemove(userId, out _);
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cierre en finally", CancellationToken.None);
            }
            Console.WriteLine($"❌ Usuario {userId} desconectado");

            //_semaphore.Release();
        }
    }

    public async Task<bool> SendMessageToUser(string userId, string message)
    {
        if (_connections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            try
            {
                var response = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando mensaje al usuario {userId}: {ex.Message}");
            }
        }
        return false;
    }
}
