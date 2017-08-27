
namespace Jabberwocky.SoC.Library
{
  using System;

  public struct ResourceClutch
  {
    public Int32 BrickCount;
    public Int32 GrainCount;
    public Int32 LumberCount;
    public Int32 OreCount;
    public Int32 WoolCount;

    public static ResourceClutch Zero = new ResourceClutch();

    public ResourceClutch (Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount)
    {
      BrickCount = brickCount;
      GrainCount = grainCount;
      LumberCount = lumberCount;
      OreCount = oreCount;
      WoolCount = woolCount;
    }

    public static Boolean operator== (ResourceClutch r1, ResourceClutch r2)
    {
      throw new NotImplementedException();
    }

    public static Boolean operator !=(ResourceClutch r1, ResourceClutch r2)
    {
      throw new NotImplementedException();
    }
  }
}
