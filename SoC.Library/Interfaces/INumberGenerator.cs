
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  public interface INumberGenerator
  {
    UInt32 RollTwoDice();

    Int32 GetRandomNumberBetweenZeroAndMaximum(UInt16 exclusiveMaximum);
  }
}
