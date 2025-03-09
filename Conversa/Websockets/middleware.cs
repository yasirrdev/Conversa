namespace Conversa.Websockets;

public class middleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            context.Request.Method = "GET";
        }

        return next(context);
    }
}
