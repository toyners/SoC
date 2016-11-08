
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

    #region Properties
    public Guid GameToken { get; private set; }
    #endregion

    public Action GameJoinedEvent;

    public Action GameLeftEvent;

    public Action GameInitializationEvent;

    #region Methods
    public void Connect()
    {
      var serviceClient = new ServiceClient();
      serviceClient.GameJoinedEvent = this.GameJoinedEventHandler;
      serviceClient.GameInitializationEvent = this.GameInitializationEvent;
      serviceClient.GameLeftEvent = this.GameLeftEventHandler;
      var instanceContext = new InstanceContext(serviceClient);
      this.serviceProviderClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      this.serviceProviderClient.TryJoinGame();
    }

    public void Disconnect()
    {
      this.serviceProviderClient.TryLeaveGame(this.GameToken);
    }

    private void GameJoinedEventHandler(Guid gameToken)
    {
      this.GameToken = gameToken;
      this.GameJoinedEvent?.Invoke();
    }

    private void GameLeftEventHandler(Guid gameToken)
    {
      this.GameToken = Guid.Empty;
      this.GameLeftEvent?.Invoke(); 
    }

    private void GameInitializationEventHandler()
    {
      this.GameInitializationEvent?.Invoke();
    }
    #endregion
  }
}
