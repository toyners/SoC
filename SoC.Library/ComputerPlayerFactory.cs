
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class ComputerPlayerFactory : IComputerPlayerFactory
  {
    public IComputerPlayer Create()
    {
      return new ComputerPlayer(Guid.NewGuid());
    }

    public IPlayer GetPlayer()
    {
      throw new NotImplementedException();
    }
  }
}
