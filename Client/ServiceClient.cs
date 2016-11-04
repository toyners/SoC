
namespace Jabberwocky.SoC.Client
{
  using System;
  using Jabberwocky.SoC.Client.ServiceReference;
  using Library;

  public class ServiceClient : IServiceProviderCallback
  {
    #region Fields
    public Action<Guid> GameJoinedEvent;

    public Action GameInitializationEvent;

    private Guid turnToken;

    private Board board;
    #endregion

    #region Methods
    public void ConfirmGameJoined(Guid gameToken)
    {
      this.GameJoinedEvent?.Invoke(gameToken);
    }

    public void GameInitialization()
    {
      this.board = new Board();
      this.GameInitializationEvent?.Invoke();
    }

    public void StartTurn(Guid turnToken)
    {
      this.turnToken = turnToken;
    }
    #endregion
  }
}
