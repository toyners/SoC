
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;

  public interface IServiceProviderCallback
  {
    [OperationContract(IsOneWay = true)]
    void StartTurn(Guid token);

    /// <summary>
    /// Confirm that the client has joined a game session.
    /// </summary>
    /// <param name="gameToken">The game identifier.</param>
    [OperationContract(IsOneWay = true)]
    void ConfirmGameSessionJoined(Guid gameToken, GameSessionManager.GameStates gameSession);

    [OperationContract(IsOneWay = true)]
    void ConfirmGameSessionReadyToLaunch();

    /// <summary>
    /// Confirm to the player that they have left the game session.
    /// </summary>
    [OperationContract(IsOneWay = true)]
    void ConfirmPlayerHasLeftGame();

    /// <summary>
    /// Confirm that another player has left the game.
    /// </summary>
    /// <param name="username">Username of player that has left game session.</param>
    [OperationContract(IsOneWay = true)]
    void ConfirmOtherPlayerHasLeftGame(String username);

    /// <summary>
    /// Instruct the client to initialize the game.
    /// </summary>
    /// <param name="gameData">The game data.</param>
    [OperationContract(IsOneWay = true)]
    void InitializeGame(GameInitializationData gameData);

    [OperationContract(IsOneWay = true)]
    void ChooseTownLocation();

    [OperationContract(IsOneWay = true)]
    void PlayerDataForJoiningClient(PlayerData playerData);

    [OperationContract(IsOneWay = true)]
    void TownPlacedDuringSetup(UInt32 locationIndex);
  }
}
