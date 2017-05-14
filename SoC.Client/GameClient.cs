
namespace Jabberwocky.SoC.Client
{
  using System;
  using System.ServiceModel;
  using Jabberwocky.SoC.Client.ServiceReference;
  using Library;
  using Library.GameBoards;

  public class GameClient
  {
    #region Fields
    private ServiceClient serviceClient;

    private ServiceProviderClient serviceProviderClient;
    #endregion

    #region Properties
    public Guid GameToken { get; private set; }

    // TODO: Check that we are connected and that game initialization has completed 
    // (i.e. Serviceclient and Board not null)
    public GameBoardManager Board { get { return this.serviceClient.Board; } }
    #endregion

    #region Events
    public event Action GameJoinedEvent;

    public event Action GameLeftEvent;

    public event Action<GameInitializationData> GameInitializationEvent;
    #endregion

    #region Methods
    public void ConfirmGameInitialization()
    {
      this.serviceProviderClient.ConfirmGameInitialized(this.GameToken);
    }

    public void Connect()
    {
      this.serviceClient = new ServiceClient();
      this.serviceClient.GameJoinedEvent = this.GameJoinedEventHandler;
      this.serviceClient.GameInitializationEvent = this.GameInitializationEventHandler;
      this.serviceClient.GameLeftEvent = this.GameLeftEventHandler;

      var instanceContext = new InstanceContext(this.serviceClient);
      this.serviceProviderClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      this.serviceProviderClient.TryJoinGame(null);
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

    private void GameInitializationEventHandler(GameInitializationData gameData)
    {
      this.GameInitializationEvent?.Invoke(gameData);
    }
    #endregion
  }
}
