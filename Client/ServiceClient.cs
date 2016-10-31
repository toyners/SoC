
namespace Jabberwocky.SoC.Client
{
  using System;
  using Jabberwocky.SoC.Client.ServiceReference;

  public class ServiceClient : IServiceProviderCallback
  {
    #region Fields
    public Action<Guid> GameJoinedEvent;

    public Action GameInitializationEvent;

    private Guid gameToken;

    private Guid turnToken;
    #endregion

    #region Methods
    public void ConfirmGameJoined(Guid gameToken)
    {
      this.gameToken = gameToken;
      this.GameJoinedEvent?.Invoke(gameToken);
    }

    public void GameInitialization()
    {
      this.GameInitializationEvent?.Invoke();
    }

    public void StartTurn(Guid turnToken)
    {
      this.turnToken = turnToken;
    }
    #endregion
  }
}
