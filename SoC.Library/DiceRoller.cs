
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class DiceRoller : IDiceRoller
  {
    private Random random = new Random();

    public UInt32 RollTwoDice() { return (UInt32)(this.random.Next(1, 6) + this.random.Next(1, 6)); }
  }
}
