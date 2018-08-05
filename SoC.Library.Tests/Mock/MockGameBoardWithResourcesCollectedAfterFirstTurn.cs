
namespace Jabberwocky.SoC.Library.UnitTests.Mock
{
  using System;
  using System.Collections.Generic;
  using GameBoards;

  /// <summary>
  /// Mock class that will return no resources when collecting as part of
  /// game setup or at start of the first turn. Resources will be collected 
  /// from the second turn onwards.
  /// </summary>
  public class MockGameBoardWithResourcesCollectedAfterFirstTurn : GameBoard
  {
    private Boolean isFirstTime = true;

    public MockGameBoardWithResourcesCollectedAfterFirstTurn() : base(BoardSizes.Standard) { }

    public override ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      return ResourceClutch.Zero;
    }

    public override Dictionary<Guid, ResourceCollection[]> GetResourcesForRoll(UInt32 diceRoll)
    {
      if (this.isFirstTime)
      {
        this.isFirstTime = false;
        return new Dictionary<Guid, ResourceCollection[]>();
      }

      return base.GetResourcesForRoll(diceRoll);
    }
  }
}
