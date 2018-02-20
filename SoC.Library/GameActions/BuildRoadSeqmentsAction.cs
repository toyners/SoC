
namespace Jabberwocky.SoC.Library.GameActions
{
  using System;
  using Enums;

  public class BuildRoadSegmentAction : ComputerPlayerAction
  {
    public readonly UInt32 StartLocation;
    public readonly UInt32 EndLocation;

    public BuildRoadSegmentAction(ComputerPlayerActionTypes action, UInt32 startLocation, UInt32 endLocation) : base(action)
    {
      this.StartLocation = startLocation;
      this.EndLocation = endLocation;
    }
  }
}
