
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Diagnostics;

  [DebuggerDisplay("{BrickCount}, {GrainCount}, {LumberCount}, {OreCount}, {WoolCount}")]
  public struct ResourceClutch
  {
    #region Fields
    public static ResourceClutch Zero = new ResourceClutch();
    public static ResourceClutch RoadSegment = new ResourceClutch(1, 0, 1, 0, 0); // 1 brick, 1 lumber
    public static ResourceClutch Settlement = new ResourceClutch(1, 1, 1, 0, 1);
    public static ResourceClutch City = new ResourceClutch(0, 2, 0, 3, 0);
    public static ResourceClutch DevelopmentCard = new ResourceClutch(0, 1, 0, 1, 1); // 1 grain, 1 ore, 1 wool
    public static ResourceClutch OneBrick = new ResourceClutch(1, 0, 0, 0, 0);
    public static ResourceClutch OneGrain = new ResourceClutch(0, 1, 0, 0, 0);
    public static ResourceClutch OneLumber = new ResourceClutch(0, 0, 1, 0, 0);
    public static ResourceClutch OneOre = new ResourceClutch(0, 0, 0, 1, 0);
    public static ResourceClutch OneWool = new ResourceClutch(0, 0, 0, 0, 1);
    public static ResourceClutch OneOfEach = new ResourceClutch(1, 1, 1, 1, 1);

    public Int32 BrickCount;
    public Int32 GrainCount;
    public Int32 LumberCount;
    public Int32 OreCount;
    public Int32 WoolCount;
    #endregion

    #region Construction
    public static ResourceClutch CreateFromResourceType(ResourceTypes resourceType)
    {
      switch (resourceType)
      {
        case ResourceTypes.Brick: return ResourceClutch.OneBrick;
        case ResourceTypes.Grain: return ResourceClutch.OneGrain;
        case ResourceTypes.Lumber: return ResourceClutch.OneLumber;
        case ResourceTypes.Ore: return ResourceClutch.OneOre;
        case ResourceTypes.Wool: return ResourceClutch.OneWool;
      }

      throw new Exception(resourceType + " not recognised.");
    }

    public ResourceClutch (Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount)
    {
      BrickCount = brickCount;
      GrainCount = grainCount;
      LumberCount = lumberCount;
      OreCount = oreCount;
      WoolCount = woolCount;
    }
    #endregion

    #region Properties
    public Int32 Count { get { return this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount; } }
    #endregion

    #region Methods
    public static ResourceClutch operator* (ResourceClutch operand1, Int32 operand2)
    {
      if (operand2 < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(operand2), operand2, "Must be a natural number");
      }

      return MultiplyByNaturalNumber(operand1, operand2);
    }

    public static ResourceClutch operator* (Int32 operand1, ResourceClutch operand2)
    {
      if (operand1 < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(operand1), operand1, "Must be a natural number");
      }

      return MultiplyByNaturalNumber(operand2, operand1);
    }

    public static Boolean operator== (ResourceClutch r1, ResourceClutch r2)
    {
      return r1.Equals(r2);
    }

    public static Boolean operator!= (ResourceClutch r1, ResourceClutch r2)
    {
      return !r1.Equals(r2);
    }

    public override Boolean Equals(Object obj)
    {
      var other = (ResourceClutch)obj;

      return this.BrickCount == other.BrickCount &&
       this.GrainCount == other.GrainCount &&
       this.LumberCount == other.LumberCount &&
       this.OreCount == other.OreCount &&
       this.WoolCount == other.WoolCount;
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
    #endregion
  }
}
