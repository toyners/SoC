
namespace Jabberwocky.SoC.Library
{
  using Interfaces;

  public class DiceFactory : IDiceFactory
  {
    public INumberGenerator Create()
    {
      return new Dice();
    }
  }
}
