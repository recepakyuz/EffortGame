using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EffortGame.RealTime
{
  public static class ClientManager
  {
    private static List<Room> rooms = new List<Room>();
    private static object token = new object();

    public static void RemoveClient(Client client)
    {
      lock (ClientManager.token)
      {
        Room room = ClientManager.rooms.FirstOrDefault<Room>((Func<Room, bool>) (r => r.RoomName == client.RoomName));
        room?.RemoveClient(client);
        if (room.clients.Any<Client>())
          return;
        ClientManager.rooms.Remove(room);
      }
    }

    public static void ReceivedMessage(Client client, string text)
    {
      Message message = (Message) JsonConvert.DeserializeObject<Message>(text);
      lock (ClientManager.token)
      {
        Room room = ClientManager.rooms.FirstOrDefault<Room>((Func<Room, bool>) (r => r.RoomName == message.Room));
        if (message.Command == "Connect")
        {
          client.RoomName = message.Room;
          client.UserName = message.User;
          if (room == null)
          {
            room = new Room() { RoomName = message.Room };
            ClientManager.rooms.Add(room);
          }
          room.AddClient(client);
        }
        else
          room.Broadcast(text);
      }
    }
  }
}
