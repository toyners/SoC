
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IComputerPlayer
  {
    Location ChooseSettlementLocation(GameBoardData gameBoardData);
    Trail ChooseRoad(GameBoardData gameBoardData);
  }
}
