
using System;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IComputerPlayer : IPlayer
  {
    #region Methods
    UInt32 ChooseSettlementLocation(GameBoardData gameBoardData);
    RoadSegment ChooseRoad(GameBoardData gameBoardData);
    ResourceClutch ChooseResourcesToDrop();
    #endregion
  }
}
