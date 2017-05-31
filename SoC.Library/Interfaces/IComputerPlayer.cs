
using System;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IComputerPlayer
  {
    Guid Id { get; }
    Location ChooseSettlementLocation(GameBoardData gameBoardData);
    Trail ChooseRoad(GameBoardData gameBoardData);
  }
}
