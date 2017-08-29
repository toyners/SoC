
namespace Jabberwocky.SoC.Library
{
  using System;

  public class ResourceProvider
  {
    #region Fields
    public readonly ResourceTypes? Type;

    Boolean HasRobber;
    #endregion

    #region Construction
    public ResourceProvider(ResourceTypes type, UInt32 productionNumber)
    {
      this.Type = type;

      if (productionNumber < 2 || productionNumber > 12)
      {
        throw new ArgumentOutOfRangeException("Parameter 'productionNumber' must be within range 2..12", (Exception)null);
      }

      this.ProductionNumber = productionNumber;
    }

    public ResourceProvider()
    {
      this.Type = null;
    }
    #endregion

    #region Properties
    public UInt32 ProductionNumber { get; private set; }
    #endregion

    #region Methods
    public static Boolean operator ==(ResourceProvider first, ResourceProvider second)
    {
      if (Object.ReferenceEquals(first, second))
      {
        return true;
      }

      if ((Object)first == null || (Object)second == null)
      {
        return false;
      }

      return (first.Type == second.Type && first.ProductionNumber == second.ProductionNumber);
    }

    public static Boolean operator !=(ResourceProvider first, ResourceProvider second)
    {
      return !(first == second);
    }

    public override Boolean Equals(Object obj)
    {
      if ((object)obj == null)
      {
        return false;
      }

      var other = obj as ResourceProvider;
      if (other == null)
      {
        return false;
      }

      if (Object.ReferenceEquals(this, obj))
      {
        return true;
      }

      return this.Type == other.Type && this.ProductionNumber == other.ProductionNumber;
    }

    public override Int32 GetHashCode()
    {
      var factor = 0;
      switch (this.Type)
      {
        case ResourceTypes.Brick: factor = 1; break;
        case ResourceTypes.Grain: factor = 10; break;
        case ResourceTypes.Lumber: factor = 100; break;
        case ResourceTypes.Ore: factor = 1000; break;
        case ResourceTypes.Wool: factor = 10000; break;
      }

      return (Int32)(this.ProductionNumber * factor); 
    }
    
    public Boolean ProducesResources(UInt32 rolledNumber) { throw new NotImplementedException(); }
    #endregion
  }
}
