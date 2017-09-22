
using System.IO;
using System.Xml;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IPlayerPool
  {
    IPlayer Create();

    /// <summary>
    /// Create a player instance from XML reader.
    /// </summary>
    /// <param name="reader">Xml reader containing player data.</param>
    /// <returns>Player instance</returns>
    IPlayer CreatePlayer(XmlReader reader);

    IPlayer GetPlayer();
  }
}
