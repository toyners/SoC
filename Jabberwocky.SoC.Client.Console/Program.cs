
namespace Jabberwocky.SoC.Client.Console
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using ServiceReference;

  public class Program
  {
    public static void Main(string[] args)
    {
      InstanceContext instanceContext = new InstanceContext(new Client());
      ServiceProviderClient service = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      Console.WriteLine("Client started");

      UInt32 clientCount = 0;
      while (true)
      {
        Console.WriteLine();
        Console.WriteLine("Press 'A' to add a client from the game");
        Console.WriteLine("Press 'R' to remove a client from the game.");
        Console.WriteLine("Press 'X' to remove all clients from the game and exit.");
        var input = Console.ReadKey();
        switch (input.Key)
        {
          case ConsoleKey.A:
          {
            TryToAddClientToGame(service, clientCount);
            break;
          }

          case ConsoleKey.R:
          {
            TryToRemoveClientFromGame(service, clientCount);
            break;
          }

          case ConsoleKey.X:
          {
            while (clientCount > 0)
            {
              TryToRemoveClientFromGame(service, clientCount);
            }

            return;
          }

          default: Console.WriteLine("Key is unrecognised"); break;
        }
      }
    }

    public static void TryToAddClientToGame(ServiceProviderClient service, UInt32 clientCount)
    {
      if (clientCount < 4)
      {
        service.JoinGame();
        clientCount++;
      }
    }

    public static void TryToRemoveClientFromGame(ServiceProviderClient service, UInt32 clientCount)
    {
      if (clientCount > 0)
      {
        service.LeaveGameAsync();
        clientCount++;
      }
    }
  }
}
