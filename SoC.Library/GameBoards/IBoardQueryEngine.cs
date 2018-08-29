
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;

  public interface IBoardQueryEngine
  {
    UInt32[] GetLocationsWithBestYield(Int32 count);
  }
}
