
namespace Jabberwocky.SoC.Client
{
  using System;
  using System.ServiceModel;
  using Jabberwocky.SoC.Client.ServiceReference;

  public class Client
  {
    private ServiceProviderClient serviceClient;

    public Boolean Connect()
    {
      var instanceContext = new InstanceContext(new ServiceClient());
      this.serviceClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      var joined = this.serviceClient.TryJoinGame();

      return joined;
    }

    public void Disconnect()
    {
      this.serviceClient.LeaveGame();
    }
  }
}
