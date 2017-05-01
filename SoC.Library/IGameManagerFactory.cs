
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
    /// <returns>Game Manager object.</returns>
    IGameManager Create();
  }
}
