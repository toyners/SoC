
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class Dice : INumberGenerator
  {
    #region Fields
    private readonly Random random = new Random();
    #endregion

    #region Methods
    public Int32 GetRandomNumberBetweenZeroAndMaximum(Int32 exclusiveMaximum)
    {
      return this.random.Next(0, exclusiveMaximum);
    }

    public void RollTwoDice(out uint dice1, out uint dice2)
    {
      dice1 = (uint)this.random.Next(1, 6);
      dice2 = (uint)this.random.Next(1, 6);
    }
    #endregion
  }
}
