
namespace Jabberwocky.SoC.Client
{
  using System;
  using Jabberwocky.SoC.Client.ServiceReference;
  using Library;

  public class ServiceClient : IServiceProviderCallback
  {
    #region Fields
    public Action<Guid> GameJoinedEvent;

    public Action<GameInitializationData> GameInitializationEvent;

    public Action GameLeftEvent;

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

    public void ConfirmGameLeft()
    {
      this.GameLeftEvent?.Invoke();
    }

    public void GameInitialization(GameInitializationData gameData)
    {
      this.Board = new Board();
      this.GameInitializationEvent?.Invoke(gameData);
    }

    public void StartTurn(Guid turnToken)
    {
      this.turnToken = turnToken;
    }
    #endregion
  }
}
