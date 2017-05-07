
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  /// <summary>
  /// Provides method for creating game manager instances 
  /// </summary>
  public interface IGameSessionManager
  {
    /// <summary>
    /// Creates a game manager instance.
    /// </summary>
    /// <returns>Game Manager object.</returns>
    IGameSession Create();
  }
}
