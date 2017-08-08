
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
    ResourceClutch ChooseResourcesToDrop();
    #endregion
  }

  public struct ResourceClutch
  {
    public Int32 BrickCount;
    public Int32 GrainCount;
    public Int32 LumberCount;
    public Int32 OreCount;
    public Int32 WoolCount;
  }

  public class ResourceBag
  {
    public Int32 BrickCount { get; private set; }
    public Int32 GrainCount { get; private set; }
    public Int32 LumberCount { get; private set; }
    public Int32 OreCount { get; private set; }
    public Int32 WoolCount { get; private set; }

    public void Add(ResourceClutch resources)
    {
      throw new NotImplementedException();
    }

    public void Remove(ResourceClutch resources)
    {
      throw new NotImplementedException();
    }

    public override Boolean Equals(Object obj)
    {
      return base.Equals(obj);
    }
  }
}
