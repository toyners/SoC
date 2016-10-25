
namespace Jabberwocky.SoC.Client
{
  using System;
  using Jabberwocky.SoC.Client.ServiceReference;

  public class ServiceClient : IServiceProviderCallback
  {
    #region Fields
    private Guid gameToken;

    private Guid turnToken;

    public Action<Guid> GameJoinedEvent;
    #endregion

    #region Methods
    public void ConfirmGameJoined(Guid gameToken)
    {
      this.gameToken = gameToken;
      this.GameJoinedEvent?.Invoke(gameToken);
    }

    public void StartTurn(Guid turnToken)
    {
      this.turnToken = turnToken;
    }
    #endregion
  }
}
