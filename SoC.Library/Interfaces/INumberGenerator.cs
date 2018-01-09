
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  public interface INumberGenerator
  {
    UInt32 RollTwoDice();

    UInt32 GetRandomNumber(Int32 exclusiveMaximum);
  }
}
