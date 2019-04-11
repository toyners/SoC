
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  public interface INumberGenerator
  {
    void RollTwoDice(out uint dice1, out uint dice2);

    int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum);
  }
}
