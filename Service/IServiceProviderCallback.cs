
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;

  public interface IServiceProviderCallback
  {
    [OperationContract(IsOneWay = true)]
    void StartTurn(Guid token);

    /// <summary>
    /// Confirmt that the client has joined a game.
    /// </summary>
    /// <param name="gameToken">The game identifier.</param>
    /// <param name="clientToken">The client identifier used when communicating to the server.</param>
    [OperationContract(IsOneWay = true)]
    void ConfirmGameJoined(Guid gameToken, Guid clientToken);

    /// <summary>
    /// Confirm that to the client that they have left the game.
    /// </summary>
    [OperationContract(IsOneWay = true)]
    void ConfirmGameLeft();

    /// <summary>
    /// Instruct the client to initialize the game.
    /// </summary>
    /// <param name="gameData">The game data.</param>
    [OperationContract(IsOneWay = true)]
    void InitializeGame(GameInitializationData gameData);

    [OperationContract(IsOneWay = true)]
    void PlaceTown();
  }
}
