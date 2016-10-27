
namespace Jabberwocky.SoC.Client
{
  using System;
  using System.ServiceModel;
  using Jabberwocky.SoC.Client.ServiceReference;

  public class GameClient
  {
    #region Fields
    private ServiceProviderClient serviceProviderClient;
    #endregion

    public Action<Guid> GameJoinedEvent;

    #region Methods
    public void Connect()
    {
      var serviceClient = new ServiceClient();
      serviceClient.GameJoinedEvent = this.GameJoinedEvent;
      var instanceContext = new InstanceContext(serviceClient);
      this.serviceProviderClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      this.serviceProviderClient.TryJoinGame();
    }

    public void Disconnect()
    {
      this.serviceProviderClient.LeaveGame();
    }
    #endregion
  }
}
