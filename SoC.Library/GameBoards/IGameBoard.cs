
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;

  public interface IGameBoard
  {
    Tuple<ResourceTypes?, uint>[] GetHexInformation();

    IBoardQueryEngine BoardQuery { get; }
  }
}
