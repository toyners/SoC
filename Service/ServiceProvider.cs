
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;
  using Logging;
  using NLog;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private GameConnector gameConnector;

    private Logger logger = LogManager.GetLogger("logfile");

    private Guid id = Guid.NewGuid();
    #endregion

    #region Construction
    public ServiceProvider()
    {
      this.logger.Info("Initiating Service Provider: " + id.ToString());
      this.gameConnector = new GameConnector();
      this.gameConnector.StartMatching();
    }
    #endregion

    #region Methods
    public void TryJoinGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.AddClient(client);
      this.logger.Info("Client joined game: " + id.ToString());
    }

    public void LeaveGame(Guid gameToken)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.RemoveClient(gameToken, client);
      this.logger.Info("Client left game");
    }
    #endregion
  }
}
