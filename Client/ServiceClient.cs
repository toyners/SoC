
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

    public Action<Guid> GameLeftEvent;

    private Guid turnToken;
    #endregion

    #region Properties
    public Board Board { get; private set; }
    #endregion

    #region Methods
    public void ConfirmGameJoined(Guid gameToken)
    {
      this.GameJoinedEvent?.Invoke(gameToken);
    }

    public void ConfirmGameLeft(Guid gameToken)
    {
      this.GameLeftEvent?.Invoke(gameToken);
    }

    public void GameInitialization()
    {
      this.Board = new Board();
      this.GameInitializationEvent?.Invoke();
    }

    public void StartTurn(Guid turnToken)
    {
      this.turnToken = turnToken;
    }
    #endregion
  }
}
