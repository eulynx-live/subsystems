using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace EulynxLive.InterlockingSystem
{
    public class WebsocketDispatcherMiddleware
    {
        private readonly RequestDelegate _next;

        public WebsocketDispatcherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, InterlockingSystem simulator)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        await simulator.HandleWebSocket(webSocket);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
