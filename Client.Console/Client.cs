
namespace Jabberwocky.SoC.Client.Console
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Jabberwocky.SoC.Client.Console.ServiceReference;

  public class Client : IServiceProviderCallback
  {
    public void StartTurn(Guid token)
    {
      Console.WriteLine(String.Format("Received token {0} for turn.", token));
    }
  }
}
