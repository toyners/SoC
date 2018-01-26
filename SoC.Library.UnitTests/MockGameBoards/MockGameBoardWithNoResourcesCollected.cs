
namespace Jabberwocky.SoC.Library.UnitTests.MockGameBoards
{
  using System;
  using System.Collections.Generic;
  using GameBoards;

  /// <summary>
  /// Mock class that will return no resources when collecting as part of
  /// game setup or during start of turn.
  /// </summary>
  public class MockGameBoardWithNoResourcesCollected : GameBoardData
  {
    public MockGameBoardWithNoResourcesCollected() : base(BoardSizes.Standard) { }

    public override ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      return ResourceClutch.Zero;
    }

    public override Dictionary<Guid, ResourceCollection[]> GetResourcesForRoll(UInt32 diceRoll)
    {
      return new Dictionary<Guid, ResourceCollection[]>();
    }
  }
}
