
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  // TODO: Change name to PlayerPool
  public class ComputerPlayerFactory : IComputerPlayerFactory
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
