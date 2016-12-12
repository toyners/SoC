
namespace Jabberwocky.SoC.Library
{
  using System;

  /// <summary>
  /// Provides method for creating game manager instances 
  /// </summary>
  public interface IGameManagerFactory
  {
    /// <summary>
    /// Creates a game manager instance.
    /// </summary>
    /// <param name="playerCount">Number of players for the game.</param>
    /// <param name="diceRollerFactory">Instance of factory that supplies dice roller for the game.</param>
    /// <returns>Game Manager object.</returns>
    IGameManager CreateGameManager(UInt32 playerCount, IDiceRollerFactory diceRollerFactory);
  }
}
