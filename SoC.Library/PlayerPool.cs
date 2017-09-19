
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.IO;
  using Interfaces;

  public class PlayerPool : IPlayerPool
  {
    public IPlayer Create()
    {
      throw new NotImplementedException();
      //return new ComputerPlayer(Guid.NewGuid());
    }

    /// <summary>
    /// Create a player instance from stream data.
    /// </summary>
    /// <param name="stream">Stream containing player data.</param>
    /// <returns>Player instance</returns>
    public IPlayer CreatePlayer(Stream stream)
    {
      throw new NotImplementedException();
    }

    public IPlayer GetPlayer()
    {
      throw new NotImplementedException();
    }
  }
}
