
namespace Jabberwocky.SoC.Client
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Jabberwocky.SoC.Client.ServiceReference;

  public class ServiceClient : IServiceProviderCallback
  {
    public void ConfirmGameJoined(Guid gameId)
    {
      throw new NotImplementedException();
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }
  }
}
