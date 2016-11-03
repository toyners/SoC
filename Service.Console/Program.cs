
namespace Jabberwocky.SoC.Service.Console
{
  using System;
  using System.ServiceModel;
  using System.Threading;
  using Logging;

  public class Program
  {
    public static void Main(String[] args)
    {
      var serviceProvider = new ServiceProvider();
      var serviceHost = new ServiceHost(typeof(ServiceProvider));

      serviceHost.Open();
      Console.WriteLine("Started...");
      Console.WriteLine("Any key to exit");
      Console.ReadKey();
      Console.WriteLine();
      Console.WriteLine("Closing...");
      serviceHost.Close();
    }
  }
}
