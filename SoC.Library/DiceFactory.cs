
namespace Jabberwocky.SoC.Library
{
  using Interfaces;

  public class DiceFactory : IDiceFactory
  {
    public IDice Create()
    {
      return new Dice();
    }
  }
}
