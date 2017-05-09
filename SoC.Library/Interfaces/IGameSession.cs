
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using GameBoards;

  /// <summary>
  /// Provides methods for managing a game.
  /// </summary>
  public interface IGameSession
  {
    #region Properties
    GameBoardManager Board { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Gets the order of the clients for the first setup pass.
    /// </summary>
    /// <returns>Returns array of player indexs in order of setup.</returns>
    UInt32[] GetFirstSetupPassOrder();

    /// <summary>
    /// Gets the order of the clients for the second setup pass.
    /// </summary>
    /// <returns>Returns array of player indexs in order of setup.</returns>
    UInt32[] GetSecondSetupPassOrder();

    /// <summary>
    /// Places town at position for the player.
    /// </summary>
    /// <param name="position">Map position index of the new town.</param>
    void PlaceTown(UInt32 position);

    /// <summary>
    /// Places road between positions for the player.
    /// </summary>
    /// <param name="playerId">Player Id of the new road.</param>
    void PlaceRoad(UInt32 playerId);

    Boolean RegisterPlayer(ClientAccount clientAccount);
    #endregion
  }
}
