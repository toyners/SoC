
namespace Jabberwocky.SoC.Client
{
  using System;
  using Jabberwocky.SoC.Client.ServiceReference;
  using Library;
  using Library.GameBoards;

  public class ServiceClient : IServiceProviderCallback
  {
    #region Fields
    public Action<Guid> GameJoinedEvent;

    public Action<GameInitializationData> GameInitializationEvent;

    public Action GameLeftEvent;

    private Guid turnToken;
    #endregion

    #region Properties
    public GameBoardManager Board { get; private set; }
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
      this.Board = new GameBoardManager(BoardSizes.Standard);
      this.GameInitializationEvent?.Invoke(gameData);
    }

    public void StartTurn(Guid turnToken)
    {
      this.turnToken = turnToken;
    }

    public void InitializeGame(GameInitializationData gameData)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown()
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameIsOver()
    {
      throw new NotImplementedException();
    }

    public void GameJoined()
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameSessionJoined(Guid gameToken, GameSessionManagerGameStates gameSession)
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameSessionReadyToLaunch()
    {
      throw new NotImplementedException();
    }

    public void ConfirmPlayerHasLeftGame()
    {
      throw new NotImplementedException();
    }

    public void ConfirmOtherPlayerHasLeftGame(String username)
    {
      throw new NotImplementedException();
    }

    public void ChooseTownLocation()
    {
      throw new NotImplementedException();
    }

    public void ReceivePersonalMessage(String sender, String text)
    {
      throw new NotImplementedException();
    }

    public void PlayerDataForJoiningClient(ServiceReference.PlayerData playerData)
    {
      throw new NotImplementedException();
    }

    public void TownPlacedDuringSetup(UInt32 locationIndex)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
