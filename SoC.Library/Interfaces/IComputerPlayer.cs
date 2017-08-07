
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public interface IComputerPlayer : IPlayer
  {
    #region Methods
    UInt32 ChooseSettlementLocation(GameBoardData gameBoardData);
    Road ChooseRoad(GameBoardData gameBoardData);
    HashSet<ResourceTypes> ChooseResourcesToDrop();
    #endregion
  }
}
