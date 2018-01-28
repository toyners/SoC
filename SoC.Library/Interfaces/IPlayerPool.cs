
using System;
using System.Xml;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IPlayerPool
  {
    /// <summary>
    /// Create a player instance.
    /// </summary>
    /// <param name="isComputer">True to create a computer player; otherwise false</param>
    /// <returns>Player instance</returns>
    IPlayer CreatePlayer(Boolean isComputer);

    /// <summary>
    /// Create a player instance from XML reader.
    /// </summary>
    /// <param name="reader">Xml reader containing player data.</param>
    /// <returns>Player instance</returns>
    IPlayer CreatePlayer(XmlReader reader);

    Guid GetBankId();
    //IPlayer GetPlayer();
  }
}
