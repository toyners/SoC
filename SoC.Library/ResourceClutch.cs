
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
      return r1.BrickCount == r2.BrickCount &&
             r1.GrainCount == r2.GrainCount &&
             r1.LumberCount == r2.LumberCount &&
             r1.OreCount == r2.OreCount &&
             r1.WoolCount == r2.WoolCount;
    }

    public static Boolean operator !=(ResourceClutch r1, ResourceClutch r2)
    {
      return !(r1 == r2);
    }

    public override Boolean Equals(Object obj)
    {
      return base.Equals(obj);
      throw new NotImplementedException();
    }
  }
}
