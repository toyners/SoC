
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

  public class ResourceBag
  {
    public Int32 BrickCount { get; private set; }
    public Int32 Count { get { return this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount; } }
    public Int32 GrainCount { get; private set; }
    public Int32 LumberCount { get; private set; }
    public Int32 OreCount { get; private set; }
    public Int32 WoolCount { get; private set; }

    public ResourceBag(Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount)
    {
      this.BrickCount = brickCount;
      this.GrainCount = grainCount;
      this.LumberCount = lumberCount;
      this.OreCount = oreCount;
      this.WoolCount = woolCount;
    }

    public ResourceBag() : this(0, 0, 0, 0, 0) { }

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
