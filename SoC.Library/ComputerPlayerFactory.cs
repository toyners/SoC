
namespace Jabberwocky.SoC.Library
{
  using Interfaces;

  public class ComputerPlayerFactory : IComputerPlayerFactory
  {
    public IComputerPlayer Create()
    {
      return new ComputerPlayer();
    }
  }
}
