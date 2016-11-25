
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.IO;
  using System.ServiceModel;
  using Logging;
  //using NLog;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Single)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private GameSessionManager gameSessionManager;
    #endregion

    #region Construction
    public ServiceProvider()
    {
      this.gameSessionManager = new GameSessionManager(new DiceRollerFactory());
      this.gameSessionManager.StartMatching();
    }
    #endregion

    #region Methods
    public void TryJoinGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameSessionManager.AddClient(client);
      Logger.Message("Client joined game");
    }

    public void TryLeaveGame(Guid gameToken)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameSessionManager.RemoveClient(gameToken, client);
      Logger.Message("Client left game");
    } 
    #endregion
  }
}
