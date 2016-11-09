
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

    public String Document { get; private set; }
    #endregion

    #region Events
    public Action GameJoinedEvent;

    public Action GameLeftEvent;

    public Action GameInitializationEvent;
    #endregion

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

    private void GameLeftEventHandler()
    {
      this.GameToken = Guid.Empty;
      this.GameLeftEvent?.Invoke();
    }

    private void GameInitializationEventHandler()
    {
      this.Document = "<html><header></header><body>Hello!<body></html>";
      this.GameInitializationEvent?.Invoke();
    }
    #endregion
  }
}
