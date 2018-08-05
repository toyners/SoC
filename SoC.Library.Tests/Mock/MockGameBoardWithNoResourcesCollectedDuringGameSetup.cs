
namespace Jabberwocky.SoC.Library.UnitTests.Mock
{
  using System;
  using GameBoards;

  /// <summary>
  /// Mock class that will return no resources when collecting as part of
  /// game setup. Resources will be collected during start of turn.
  /// </summary>
  public class MockGameBoardWithNoResourcesCollectedDuringGameSetup : GameBoard
  {
    public MockGameBoardWithNoResourcesCollectedDuringGameSetup() : base(BoardSizes.Standard) { }

    public override ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      return ResourceClutch.Zero;
    }
  }
}
