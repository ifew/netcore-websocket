using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace api.WebSocketManager
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<string, List<string>> _groups = new ConcurrentDictionary<string, List<string>>();

        public WebSocket GetSocketById(string id)
        {
            System.Console.WriteLine($"Get Socket by ID: {id}");
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            System.Console.WriteLine($"Get All Socket");
            return _sockets;
        }

        public ConcurrentDictionary<string, List<string>> GetGroups()
        {
            System.Console.WriteLine($"Get All Socket");
            return _groups;
        }

        public List<string> GetAllFromGroup(string GroupID)
        {
            System.Console.WriteLine($"Get Socket from Group ID: {GroupID}");
 
            if (_groups.ContainsKey(GroupID))
            {
                return _groups[GroupID];
            }

            return default(List<string>);
        }

        public string GetId(WebSocket socket)
        {
            System.Console.WriteLine($"Get ID by Socket");
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AddSocket(WebSocket socket)
        {
            _sockets.TryAdd(CreateConnectionId(), socket);
        }

        public void AddToGroup(string socketID, string groupID)
        {
            if (_groups.ContainsKey(groupID))
            {
                _groups[groupID].Add(socketID);

                return;
            }

            _groups.TryAdd(groupID, new List<string> { socketID });
            System.Console.WriteLine($"Added Socket ID {socketID} to Group ID {groupID}");
        }

        public void RemoveFromGroup(string socketID, string groupID)
        {
            if (_groups.ContainsKey(groupID))
            {
                _groups[groupID].Remove(socketID);
            }
            System.Console.WriteLine($"Removed Socket ID {socketID} to Group ID {groupID}");
        }

        public async Task RemoveSocket(string id)
        {
            if (id == null) return;

            WebSocket socket;
            _sockets.TryRemove(id, out socket);

            if (socket.State != WebSocketState.Open) return;

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketManager",
                                    cancellationToken: CancellationToken.None).ConfigureAwait(false);

            System.Console.WriteLine($"Removed Socket ID : {id}");
        }

        private string CreateConnectionId()
        {
            var ConnectionId = Guid.NewGuid().ToString();
            System.Console.WriteLine($"Created ConnectionID : {ConnectionId}");
            return ConnectionId;
        }
    }
}