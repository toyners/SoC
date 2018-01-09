
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class Dice : INumberGenerator
  {
    private Random random = new Random();

    public UInt32 GetRandomNumber(Int32 exclusiveMaximum)
    {
      return (UInt32)this.random.Next(0, exclusiveMaximum);
    }

    public UInt32 RollTwoDice() { return (UInt32)(this.random.Next(1, 6) + this.random.Next(1, 6)); }
  }
}
