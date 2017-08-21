
namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IPlayerPool
  {
    IPlayer Create();

    IPlayer GetPlayer();
  }
}
