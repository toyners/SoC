
namespace Jabberwocky.SoC.Client.Console
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;
  using ServiceReference;

  public class Program
  {
    private static List<ServiceProviderClient> serviceClients = new List<ServiceProviderClient>();

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
            while (serviceClients.Count > 0)
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
      var instanceContext = new InstanceContext(new Client());
      var serviceClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      var gameJoined = serviceClient.JoinGame();

      if (gameJoined)
      {
        serviceClients.Add(serviceClient);
        Console.WriteLine("Joined.");
      }
      else
      {
        Console.WriteLine("FAILED!");
      }
    }

    public static void TryToRemoveClientFromGame()
    {
      serviceClients[serviceClients.Count - 1].LeaveGame();
      serviceClients.RemoveAt(serviceClients.Count - 1);
      Console.WriteLine("Client removed.");
    }
  }
}
