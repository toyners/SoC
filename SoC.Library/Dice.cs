
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class Dice : INumberGenerator
  {
    #region Fields
    private Random random = new Random();
    #endregion

    #region Methods
    public Int32 GetRandomNumberBetweenZeroAndMaximum(Int32 exclusiveMaximum)
    {
      return this.random.Next(0, exclusiveMaximum);
    }

    public UInt32 RollTwoDice() { return (UInt32)(this.random.Next(1, 6) + this.random.Next(1, 6)); }
    #endregion
  }
}
