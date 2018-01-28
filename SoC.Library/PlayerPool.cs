
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Xml;
  using Interfaces;

  public class PlayerPool : IPlayerPool
  {
    private Guid bankId = Guid.NewGuid();

    /// <summary>
    /// Create a player instance.
    /// </summary>
    /// <param name="isComputer">True to create a computer player; otherwise false</param>
    /// <returns>Player instance</returns>
    public IPlayer CreatePlayer(Boolean isComputer)
    {
      if (isComputer)
      {
        return new ComputerPlayer();
      }

      return new Player();
    }

    /// <summary>
    /// Create a player instance from XML reader.
    /// </summary>
    /// <param name="reader">Xml reader containing player data.</param>
    /// <returns>Player instance</returns>
    public IPlayer CreatePlayer(XmlReader reader)
    {
      var isComputer = false;
      var isComputerValue = reader.GetAttribute("iscomputer");
      if (!String.IsNullOrEmpty(isComputerValue))
      {
        if (Boolean.TryParse(isComputerValue, out isComputer) && isComputer)
        {
          var computerPlayer = new ComputerPlayer();
          computerPlayer.Load(reader);
          return computerPlayer;
        }
      }

      var player = new Player();
      player.Load(reader);
      return player;
    }

    /// <summary>
    /// Get the id of the bank
    /// </summary>
    /// <returns>Bank id</returns>
    public Guid GetBankId()
    {
      return this.bankId;
    }
  }
}
