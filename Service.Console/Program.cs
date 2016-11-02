
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
      //var serviceHost = new ServiceHost(typeof(ServiceProvider));
      var serviceHost = new ServiceHost(serviceProvider);

      serviceHost.Open();
      Console.WriteLine("Started...");

      while (true)
      {
        Thread.Sleep(250);
      }
      
      //Console.WriteLine("Any key to exit");
      //Console.ReadKey();
      //Console.WriteLine();
      //Logger.Message("Closing...");
      //serviceHost.Close();
    }
  }
}
