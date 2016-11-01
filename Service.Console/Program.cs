
namespace Jabberwocky.SoC.Service.Console
{
  using System;
  using System.ServiceModel;
  using Logging;

  public class Program
  {
    public static void Main(String[] args)
    {
      Logger.MessageEvent = (message) => { Console.WriteLine(message); };
      var serviceHost = new ServiceHost(typeof(ServiceProvider));

      serviceHost.Open();
      Logger.Message("Started...");
      Console.WriteLine("Any key to exit");
      Console.ReadKey();

      Logger.Message("Closing...");
      serviceHost.Close();
    }
  }
}
