
namespace Jabberwocky.SoC.Library.GameActions
{
  using System;
  using System.Collections.Generic;
  using Enums;

  public class BuildRoadAction : ComputerPlayerAction
  {
    private UInt32[] locations;

    public BuildRoadAction(ComputerPlayerActionTypes action, UInt32[] locations) : base(action)
    {
      this.locations = locations;
    }

    public IEnumerable<Tuple<UInt32, UInt32>> RoadSegments()
    {
      for (var index = 0; index < this.locations.Length; index += 2)
      {
        var segmentStartLocation = this.locations[index];
        var segmentEndLocation = this.locations[index + 1];
        yield return new Tuple<UInt32, UInt32>(segmentStartLocation, segmentEndLocation);
      }

      yield break;
    }
  }
}
