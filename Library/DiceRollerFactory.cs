
namespace Jabberwocky.SoC.Service
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
