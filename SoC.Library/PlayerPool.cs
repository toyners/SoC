
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class PlayerPool : IPlayerPool
  {
    public IPlayer Create()
    {
      throw new NotImplementedException();
      //return new ComputerPlayer(Guid.NewGuid());
    }

    public IPlayer GetPlayer()
    {
      throw new NotImplementedException();
    }
  }
}
