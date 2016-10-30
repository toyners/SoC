
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
      Logger.MessageEvent = (message) => { Console.WriteLine(message); };
      var serviceHost = new ServiceHost(typeof(ServiceProvider));

      serviceHost.Open();
      Logger.Message("Started...");

      while (true)
      {
        Thread.Sleep(100);
      }
    }
  }
}
