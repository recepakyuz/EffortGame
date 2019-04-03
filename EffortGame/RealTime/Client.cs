using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EffortGame.RealTime
{
  public class Client
  {
    private WebSocket webSocket;

    public string UserName { get; set; }

    public string RoomName { get; set; }

    public string Key { get; set; }

    public Client(WebSocket _webSocket)
    {
      this.webSocket = _webSocket;
      this.Key = Guid.NewGuid().ToString();
      this.Send(JsonConvert.SerializeObject((object) new Message()
      {
        Command = "Connected",
        Value = this.Key
      }));
    }

    public async Task Receive()
    {
      Client client = this;
      byte[] bag = new byte[1024];
      WebSocketReceiveResult async;
      for (async = await client.webSocket.ReceiveAsync(new ArraySegment<byte>(bag), CancellationToken.None); !async.CloseStatus.HasValue; async = await client.webSocket.ReceiveAsync(new ArraySegment<byte>(bag), CancellationToken.None))
      {
        string text = Encoding.UTF8.GetString(bag, 0, async.Count);
        ClientManager.ReceivedMessage(client, text);
      }
      await client.webSocket.CloseAsync(async.CloseStatus.Value, async.CloseStatusDescription, CancellationToken.None);
      ClientManager.RemoveClient(client);
    }

    public async Task Send(string message)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(message);
      await this.webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }
  }
}
