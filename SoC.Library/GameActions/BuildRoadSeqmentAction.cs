
namespace Jabberwocky.SoC.Library.GameActions
{
  using System;
  using System.Collections.Generic;
  using Enums;

  public class BuildRoadAction : ComputerPlayerAction
  {
    public UInt32[] Locations;

    public BuildRoadAction(ComputerPlayerActionTypes action, UInt32[] locations) : base(action)
    {
      this.Locations = locations;
    }
  }
}
