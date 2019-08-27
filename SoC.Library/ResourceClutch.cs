
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("B{BrickCount}, G{GrainCount}, L{LumberCount}, O{OreCount}, W{WoolCount}")]
    public struct ResourceClutch
    {
        #region Fields
        public static ResourceClutch Zero = new ResourceClutch();
        public static ResourceClutch RoadSegment = new ResourceClutch(1, 0, 1, 0, 0); // 1 brick, 1 lumber
        public static ResourceClutch Settlement = new ResourceClutch(1, 1, 1, 0, 1); // 1 brick, 1 grain, 1 lumber, 1 wool
        public static ResourceClutch City = new ResourceClutch(0, 2, 0, 3, 0); // 2 grain, 3 ore
        public static ResourceClutch DevelopmentCard = new ResourceClutch(0, 1, 0, 1, 1); // 1 grain, 1 ore, 1 wool
        public static ResourceClutch OneBrick = new ResourceClutch(1, 0, 0, 0, 0);
        public static ResourceClutch OneGrain = new ResourceClutch(0, 1, 0, 0, 0);
        public static ResourceClutch OneLumber = new ResourceClutch(0, 0, 1, 0, 0);
        public static ResourceClutch OneOre = new ResourceClutch(0, 0, 0, 1, 0);
        public static ResourceClutch OneWool = new ResourceClutch(0, 0, 0, 0, 1);
        public static ResourceClutch OneOfEach = new ResourceClutch(1, 1, 1, 1, 1);
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

        public ResourceClutch(int brickCount, int grainCount, int lumberCount, int oreCount, int woolCount)
        {
            this.BrickCount = brickCount;
            this.GrainCount = grainCount;
            this.LumberCount = lumberCount;
            this.OreCount = oreCount;
            this.WoolCount = woolCount;
        }

        public ResourceClutch(ResourceClutch r)
        {
            this.BrickCount = r.BrickCount;
            this.GrainCount = r.GrainCount;
            this.LumberCount = r.LumberCount;
            this.OreCount = r.OreCount;
            this.WoolCount = r.WoolCount;
        }
        #endregion

        #region Properties
        public int BrickCount { get; }
        public int Count { get { return this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount; } }
        public int GrainCount { get; }
        public int LumberCount { get; }
        public int OreCount { get; }
        public int WoolCount { get; }
        #endregion

        #region Methods
        public static ResourceClutch operator *(ResourceClutch operand1, int operand2)
        {
            if (operand2 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(operand2), operand2, "Must be a natural number");
            }

            return MultiplyByNaturalNumber(operand1, operand2);
        }

        public static ResourceClutch operator *(int operand1, ResourceClutch operand2)
        {
            if (operand1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(operand1), operand1, "Must be a natural number");
            }

            return MultiplyByNaturalNumber(operand2, operand1);
        }

        public static bool operator ==(ResourceClutch r1, ResourceClutch r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(ResourceClutch r1, ResourceClutch r2)
        {
            return !r1.Equals(r2);
        }

        public static bool operator <=(ResourceClutch r1, ResourceClutch r2)
        {
            return r1.BrickCount <= r2.BrickCount &&
              r1.GrainCount <= r2.GrainCount &&
              r1.LumberCount <= r2.LumberCount &&
              r1.OreCount <= r2.OreCount &&
              r1.WoolCount <= r2.WoolCount;
        }

        public static bool operator >(ResourceClutch r1, ResourceClutch r2)
        {
            return !(r1 <= r2);
        }

        public static bool operator >=(ResourceClutch r1, ResourceClutch r2)
        {
            return r1.BrickCount >= r2.BrickCount &&
              r1.GrainCount >= r2.GrainCount &&
              r1.LumberCount >= r2.LumberCount &&
              r1.OreCount >= r2.OreCount &&
              r1.WoolCount >= r2.WoolCount;
        }

        public static bool operator <(ResourceClutch r1, ResourceClutch r2)
        {
            return !(r1 >= r2);
        }

        public static ResourceClutch operator -(ResourceClutch r1, ResourceClutch r2)
        {
            return new ResourceClutch(r1.BrickCount - r2.BrickCount,
              r1.GrainCount - r2.GrainCount,
              r1.LumberCount - r2.LumberCount,
              r1.OreCount - r2.OreCount,
              r1.WoolCount - r2.WoolCount);
        }

        public static ResourceClutch operator +(ResourceClutch r1, ResourceClutch r2)
        {
            return new ResourceClutch(r1.BrickCount + r2.BrickCount,
              r1.GrainCount + r2.GrainCount,
              r1.LumberCount + r2.LumberCount,
              r1.OreCount + r2.OreCount,
              r1.WoolCount + r2.WoolCount);
        }

        public override bool Equals(Object obj)
        {
            var other = (ResourceClutch)obj;

            return this.BrickCount == other.BrickCount &&
             this.GrainCount == other.GrainCount &&
             this.LumberCount == other.LumberCount &&
             this.OreCount == other.OreCount &&
             this.WoolCount == other.WoolCount;
        }

        private static ResourceClutch MultiplyByNaturalNumber(ResourceClutch operand1, int operand2)
        {
            return new ResourceClutch(
              operand1.BrickCount * operand2,
              operand1.GrainCount * operand2,
              operand1.LumberCount * operand2,
              operand1.OreCount * operand2,
              operand1.WoolCount * operand2);
        }

        public override int GetHashCode()
        {
            var hashCode = -2128923598;
            hashCode = hashCode * -1521134295 + this.BrickCount.GetHashCode();
            hashCode = hashCode * -1521134295 + this.GrainCount.GetHashCode();
            hashCode = hashCode * -1521134295 + this.LumberCount.GetHashCode();
            hashCode = hashCode * -1521134295 + this.OreCount.GetHashCode();
            hashCode = hashCode * -1521134295 + this.WoolCount.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            var result = "";

            if (this.BrickCount > 0)
                result += $"{this.BrickCount} Brick, ";

            if (this.GrainCount > 0)
                result += $"{this.GrainCount} Grain, ";

            if (this.LumberCount > 0)
                result += $"{this.LumberCount} Lumber, ";

            if (this.OreCount > 0)
                result += $"{this.OreCount} Ore, ";

            if (this.WoolCount > 0)
                result += $"{this.WoolCount} Wool";

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            else if (result.Length == 0)
                result = "(nothing)";

            return result;
        }
        #endregion
    }
}
