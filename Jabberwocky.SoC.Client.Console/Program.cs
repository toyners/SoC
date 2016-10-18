
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

      service.JoinGame();

      Thread.Sleep(1000);

      service.LeaveGame();

      Console.ReadKey();
    }
  }
}
