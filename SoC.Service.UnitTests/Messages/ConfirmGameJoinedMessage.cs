
namespace Service.UnitTests.Messages
{
  using System;
  using Jabberwocky.SoC.Service;

  public class ConfirmGameJoinedMessage : MessageBase
  {
    #region Construction
    public ConfirmGameJoinedMessage(Guid gameToken, GameSessionManager.GameStates gameState)
    {
      this.GameToken = gameToken;
      this.GameState = gameState;
    }
    #endregion

    #region Properties
    public Object GameState { get; internal set; }
    public Guid GameToken { get; private set; }
    #endregion
  }
}
