
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;
  using Logging;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private GameConnector gameConnector;
    #endregion

    #region Construction
    public ServiceProvider()
    {
      Logger.Message("Initiating Service Provider");
      this.gameConnector = new GameConnector();
      this.gameConnector.StartMatching();
    }
    #endregion

    #region Methods
    public void TryJoinGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.AddClient(client);
      Logger.Message("Client joined game");
    }

    public void LeaveGame(Guid gameToken)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.RemoveClient(gameToken, client);
      Logger.Message("Client left game");
    }
    #endregion
  }
}
