
namespace Jabberwocky.SoC.Library.Interfaces
{
  // TODO: Rename to IPlayerPool
  public interface IComputerPlayerFactory
  {
    IPlayer Create();

    IPlayer GetPlayer();
  }
}
