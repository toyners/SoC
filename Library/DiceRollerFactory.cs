
namespace Jabberwocky.SoC.Library
{
  using System;
  using Library;

  public class DiceRollerFactory : IDiceRollerFactory
  {
    public IDiceRoller Create()
    {
      return new DiceRoller();
    }
  }
}
