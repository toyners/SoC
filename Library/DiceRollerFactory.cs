
namespace Jabberwocky.SoC.Service
{
  using System;
  using Library;

  public class DiceRollerFactory : IDiceRollerFactory
  {
    public DiceRoller Create()
    {
      return new DiceRoller();
    }
  }
}
