
using System;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IComputerPlayer : IPlayer
  {
    UInt32 ChooseSettlementLocation(GameBoardData gameBoardData);
    Trail ChooseRoad(GameBoardData gameBoardData);
  }
}
