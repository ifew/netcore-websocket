using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.WebSocketManager;
using api.Contexts;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class BroadcastController : Controller
    {
        private  ChatContext _chatContext { get; set; }
        public BroadcastController(ChatContext chatContext)
        {
            _chatContext = chatContext;
        }

        //List all connections
        [HttpGet]
        public IEnumerable<string> List()
        {
            List<string> list = new List<string>();
            foreach (var connection in  _chatContext.WebSocketConnectionManager.GetAll())
            {
                if (connection.Value.State == WebSocketState.Open) {
                    list.Add(_chatContext.WebSocketConnectionManager.GetId(connection.Value));
                }
            }
            return list;
        }

        //Send message to all connection
        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<string>> SendToAll()
        {
            await _chatContext.SendMessageToAllAsync("Hello Everyone");
            return new string[] { "Broadcast hello message to all" };
        }

        //Send message to connection id
        [HttpGet("{connection_id}")]
        public async Task<string> Send(string connection_id)
        {
            await _chatContext.SendMessageAsync(connection_id, $"Hello {connection_id}");
            return $"Send message 'Hello' to {connection_id}";
        }

        //Send custom message to connection id
        [HttpPost("{connection_id}")]
        public async Task<string> SendMessage(string connection_id, [FromBody] MessageModel json)
        {
            await _chatContext.SendMessageAsync(connection_id, $"{json.message}");
            return $"Send message '{json.message}' to {connection_id}";
        }

        //Get all groups
        [HttpGet("group")]
        public IEnumerable<string> ListGroups(string group_id)
        {
            List<string> list = new List<string>();
            foreach (var connection in  _chatContext.WebSocketConnectionManager.GetGroups())
            {
                list.Add(connection.Key);
            }
            return list;
        }

        //Get connection in group
        [HttpGet("group/{group_id}")]
        public IEnumerable<string> ListConnectionsInGroup(string group_id)
        {
            List<string> list = _chatContext.WebSocketConnectionManager.GetAllFromGroup(group_id);
            return list;
        }

        //Add group connection
        [HttpPost]
        [Route("group")]
        public JsonResult CreateGroup([FromBody] GroupModel json)
        {
            _chatContext.WebSocketConnectionManager.AddToGroup(json.connection_id, json.group_id);
            return Json(json);
        }

        //Send message to group connection
        [HttpPost]
        [Route("group/{group_id}")]
        public async Task<JsonResult> SendToGroup(string group_id, [FromBody] MessageModel json)
        {
            await _chatContext.SendMessageToGroupAsync(group_id, json.message);
            return Json(json);
        }

        //Send message to group connection
        [HttpDelete]
        [Route("group/{group_id}/{connection_id}")]
        public string DeleteFromGroup(string group_id, string connection_id)
        {
            _chatContext.WebSocketConnectionManager.RemoveFromGroup(connection_id, group_id);
            return $"Removed Connection ID '{connection_id}' from Group ID {group_id}";
        }

    }

    public class MessageModel
    {
        public string message { get; set; }
    }

    public class GroupModel {
        public string connection_id { get; set; }
        public string group_id { get; set; }
    }

    public class GroupListModel {
        public string connection_id { get; set; }
    }
}