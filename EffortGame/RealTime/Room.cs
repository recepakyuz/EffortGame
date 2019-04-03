using System.Collections.Generic;

namespace EffortGame.RealTime
{
  public class Room
  {
    public List<Client> clients = new List<Client>();
    private object token = new object();

    public string RoomName { get; set; }

    public void AddClient(Client client)
    {
      lock (this.token)
        this.clients.Add(client);
    }

    public void Broadcast(string message)
    {
      lock (this.token)
      {
        foreach (Client client in this.clients)
          client.Send(message);
      }
    }

    public void RemoveClient(Client client)
    {
      lock (this.token)
        this.clients.Remove(client);
    }
  }
}
