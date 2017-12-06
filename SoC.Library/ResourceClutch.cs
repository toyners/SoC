
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
    public static ResourceClutch RoadSegment = new ResourceClutch(1, 0, 1, 0, 0);
    public static ResourceClutch Settlement = new ResourceClutch(1, 1, 1, 0, 1);

    public ResourceClutch (Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount)
    {
      BrickCount = brickCount;
      GrainCount = grainCount;
      LumberCount = lumberCount;
      OreCount = oreCount;
      WoolCount = woolCount;
    }

    public static ResourceClutch operator* (ResourceClutch operand1, Int32 operand2)
    {
      if (operand2 < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(operand2), operand2, "Must be a natural number");
      }

      return MultiplyByNaturalNumber(operand1, operand2);
    }

    public static ResourceClutch operator *(Int32 operand1, ResourceClutch operand2)
    {
      if (operand1 < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(operand1), operand1, "Must be a natural number");
      }

      return MultiplyByNaturalNumber(operand2, operand1);
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
    }

    private static ResourceClutch MultiplyByNaturalNumber(ResourceClutch operand1, Int32 operand2)
    {
      return new ResourceClutch(
        operand1.BrickCount * operand2,
        operand1.GrainCount * operand2,
        operand1.LumberCount * operand2,
        operand1.OreCount * operand2,
        operand1.WoolCount * operand2);
    }
  }
}
