# Example .Net Core 2 Web Socket

Based from 

- https://github.com/AltairCA/.netcore2.0-websocketServerExample/tree/master/WebSocketDemo
- https://github.com/radu-matei/websocket-manager/tree/master/src/WebSocketManager

## Usage

Start Websocket Server
```shell
$ cd src
$ dotnet run
```

Download websocket tools such as
- https://chrome.google.com/webstore/detail/simple-websocket-client/pfdhoblngboilpfeibdedpjgfnlcodoo?hl=en

### Client URL
ws://localhost:5000/chat

### Get All Connections
GET http://localhost:5001/api/broadcast

### Broadcast to all
GET http://localhost:5001/api/broadcast/all

### Broadcast to Connection ID with Hello text
GET https://localhost:5000/api/broadcast/{CONNECTION_ID}

### Broadcast to Connection ID wuth custom text
POST https://localhost:5000/api/broadcast/{CONNECTION_ID}

```json
{
   "message" : "Hi, there"
}
```

### Create Group with add connection id to
POST https://localhost:5000/api/broadcast/group/new

```json
{
    "connection_id" : "{CONNECTION_ID}",
    "group_id": "{GROUP_NAME}"
}
```

### Get All Groups
GET https://localhost:5000/api/broadcast/group

### Broadcast to Group ID
POST https://localhost:5000/api/broadcast/group/{GROUP_NAME}

```json
{
	"message" : "hello secret"
}
```

### Delete Connection ID from Group
DELETE https://localhost:5000/api/broadcast/group/{GROUP_NAME}/{CONNECTION_ID}