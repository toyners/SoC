
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;

  public class MockGameBoardData : GameBoardData
  {
    public MockGameBoardData() : base(BoardSizes.Standard) { }

    public override ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      return ResourceClutch.Zero;
    }

    public override Dictionary<Guid, ResourceCollection[]> GetResourcesForRoll(UInt32 diceRoll)
    {
      return new Dictionary<Guid, ResourceCollection[]>();
    }
  }

  public class MockGameBoardData2 : GameBoardData
  {
    public MockGameBoardData2() : base(BoardSizes.Standard) { }

    public override ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      return ResourceClutch.Zero;
    }
  }
}
