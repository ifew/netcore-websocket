using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace api.WebSocketManager
{
    public abstract class WebSocketHandler
    {
        public WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);

            var connectionID = WebSocketConnectionManager.GetId(socket);
            
            var message = $"Connected ID: \"{connectionID}\"";
            await SendMessageAsync(connectionID, message);

            System.Console.WriteLine(message);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var connectionID = WebSocketConnectionManager.GetId(socket);

            var message = $"Disconnected ID: \"{connectionID}\"";
            await SendMessageAsync(connectionID, message);

            await WebSocketConnectionManager.RemoveSocket(connectionID);

            System.Console.WriteLine($"Disconnected ID: \"{connectionID}\"");
       
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.UTF8.GetBytes(message),
                                                                  offset: 0,
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);

            System.Console.WriteLine($"Sent Message to Socket package ID: \"{WebSocketConnectionManager.GetId(socket)}\" with message \"{message}\"");
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            try
            {
                await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
                System.Console.WriteLine($"Sent Message to Socket ID: \"{socketId}\" with message \"{message}\"");
            }
            catch (Exception)
            {

            }

        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                try
                {
                    if (pair.Value.State == WebSocketState.Open)
                        await SendMessageAsync(pair.Value, message).ConfigureAwait(false);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        await OnDisconnected(pair.Value);
                    }
                }
            }
        }

        public async Task SendMessageToGroupAsync(string groupID, string message)
        {
            var sockets = WebSocketConnectionManager.GetAllFromGroup(groupID);
            if (sockets != null)
            {
                foreach (var socket in sockets)
                {
                    await SendMessageAsync(socket, message);
                }
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}