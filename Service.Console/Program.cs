
namespace Jabberwocky.SoC.Service.Console
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public class Program
  {
    public static void Main(String[] args)
    {
      //var serviceProvider = new ServiceProvider();
      var serviceHost = new ServiceHost(typeof(ServiceProvider));
      serviceHost.Open();
      Console.WriteLine("Started...");

      while (true)
      {
        if (ServiceProvider.Message != null)
        {
          Console.WriteLine(ServiceProvider.Message);
          ServiceProvider.Message = null;
        }

        Thread.Sleep(100);
      }
    }
  }
}
