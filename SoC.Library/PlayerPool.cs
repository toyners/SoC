
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Xml;
  using Interfaces;

  public class PlayerPool : IPlayerPool
  {
    public IPlayer Create()
    {
      throw new NotImplementedException();
      //return new ComputerPlayer(Guid.NewGuid());
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

    public IPlayer GetPlayer()
    {
      throw new NotImplementedException();
    }
  }
}
