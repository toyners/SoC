
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;

  public class MockPlayer : Player
  {
    public MockPlayer(String name) : base(name) {}

    public void RemoveAllResources()
    {
      var resources = new ResourceClutch(this.BrickCount, this.GrainCount, this.LumberCount, this.OreCount, this.WoolCount);
      this.RemoveResources(resources);
    }
  }
}
