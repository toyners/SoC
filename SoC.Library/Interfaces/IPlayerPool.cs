
using System;
using System.Xml;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IPlayerPool
  {
    /// <summary>
    /// Create a computer player instance
    /// </summary>
    /// <param name="gameBoard">Game board instance for use in decision making.</param>
    /// <returns>Computer player instance.</returns>
    IPlayer CreateComputerPlayer(GameBoard gameBoard);
    
    /// <summary>
    /// Create a player instance.
    /// </summary>
    /// <returns>Player instance</returns>
    IPlayer CreatePlayer();

    /// <summary>
    /// Create a player instance from XML reader.
    /// </summary>
    /// <param name="reader">Xml reader containing player data.</param>
    /// <returns>Player instance</returns>
    IPlayer CreatePlayer(XmlReader reader);

    /// <summary>
    /// Get the id of the bank
    /// </summary>
    /// <returns>Bank id</returns>
    Guid GetBankId();
  }
}
