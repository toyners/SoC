
using System.IO;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IPlayerPool
  {
    IPlayer Create();

    /// <summary>
    /// Create a player instance from stream data.
    /// </summary>
    /// <param name="stream">Stream containing player data.</param>
    /// <returns>Player instance</returns>
    IPlayer CreatePlayer(Stream stream);

    IPlayer GetPlayer();
  }
}
