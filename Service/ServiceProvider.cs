
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;
  using Library;
  using Logging;

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
      this.gameSessionManager = new GameSessionManager(1, @"C:\Projects\SOC_Logging\");
      this.gameSessionManager.PlayerCardRepository = new PlayerCardRepository();
      this.gameSessionManager.Start();
    }
    #endregion

    #region Methods
    public void ConfirmGameInitialized(Guid gameToken)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameSessionManager.ConfirmGameInitialized(gameToken, client);
      throw new NotImplementedException();
    }

    public void SendInstructions()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();

      throw new NotImplementedException();
    }

    public void TryJoinGame(String username)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameSessionManager.AddPlayer(client, username);
      ServiceLogger.Message("Client joined game");
    }

    public void TryLeaveGame(Guid gameToken)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameSessionManager.RemoveClient(gameToken, client);
      ServiceLogger.Message("Client left game");
    }
    #endregion
  }
}
