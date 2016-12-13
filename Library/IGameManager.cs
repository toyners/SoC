
namespace Jabberwocky.SoC.Library
{
  using System;

  /// <summary>
  /// Provides methods for managing a game.
  /// </summary>
  public interface IGameManager
  {
    #region Properties
    Board Board { get; }
    #endregion

    #region Methods
    UInt32[] GetFirstSetupPassOrder();

    /// <summary>
    /// Places town at position for the player.
    /// </summary>
    /// <param name="playerId">Player Id of the new town.</param>
    void PlaceTown(UInt32 playerId);

    /// <summary>
    /// Places road between positions for the player.
    /// </summary>
    /// <param name="playerId">Player Id of the new road.</param>
    void PlaceRoad(UInt32 playerId);
    #endregion
  }
}
