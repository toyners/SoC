
namespace Jabberwocky.SoC.Client.Console
{
  using System;
  using System.Collections.Generic;

  public class Program
  {
    private static List<GameClient> clients = new List<GameClient>();

    public static void Main(string[] args)
    {
      while (true)
      {
        Console.WriteLine();
        Console.WriteLine("Press 'A' to add a player to the game");
        Console.WriteLine("Press 'R' to remove a player from the game.");
        Console.WriteLine("Press 'X' to remove all players from the game and exit.");
        var input = Console.ReadLine();
        switch (input.ToUpper())
        {
          case "A":
          {
            TryToAddClientToGame();
            break;
          }

          case "R":
          {
            TryToRemoveClientFromGame();
            break;
          }

          case "X":
          {
            while (clients.Count > 0)
            {
              TryToRemoveClientFromGame();
            }

            return;
          }

          default: Console.WriteLine("Key is unrecognised"); break;
        }
      }
    }

    public static void TryToAddClientToGame()
    {
      Console.Write("Attempting connection...");
      var client = new GameClient();
      var joined = client.Connect();

      if (joined)
      {
        clients.Add(client);
        Console.WriteLine("Joined. Awaiting confirmation");
      }
      else
      {
        Console.WriteLine("FAILED!");
      }
    }

    public static void TryToRemoveClientFromGame()
    {
      if (clients.Count > 0)
      {
        clients[clients.Count - 1].Disconnect();
        clients.RemoveAt(clients.Count - 1);
        Console.WriteLine("Client removed.");
      }
    }
  }
}
