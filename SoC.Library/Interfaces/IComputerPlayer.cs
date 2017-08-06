
using System;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IComputerPlayer : IPlayer
  {
    UInt32 ChooseSettlementLocation(GameBoardData gameBoardData);
    Road ChooseRoad(GameBoardData gameBoardData);
    void DropResources(Int32 resourceCount);
  }
}
