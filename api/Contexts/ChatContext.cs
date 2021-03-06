using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using api.WebSocketManager;

namespace api.Contexts
{
    public class ChatContext : WebSocketHandler
    {
        public ChatContext(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {

        }
        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await SendMessageToAllAsync(message);
            System.Console.WriteLine($"Send message to All from context: {message}");
        }
    }
}