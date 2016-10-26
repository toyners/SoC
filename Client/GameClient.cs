
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

    #region Methods
    public Boolean Connect()
    {
      var serviceClient = new ServiceClient();
      serviceClient.GameJoinedEvent = this.GameJoinedEventHandler;
      var instanceContext = new InstanceContext(serviceClient);
      this.serviceProviderClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      this.serviceProviderClient.TryJoinGame();

      return true;
    }

    public void GameJoinedEventHandler(Guid gameId)
    {
      Console.WriteLine("Game joined confirmed: " + gameId);
    }

    public void Disconnect()
    {
      this.serviceProviderClient.LeaveGame();
    }
    #endregion
  }
}
