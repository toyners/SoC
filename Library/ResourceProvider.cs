
namespace Jabberwocky.SoC.Library
{
  using System;

  public class ResourceProvider
  {
    #region Fields
    public readonly ResourceTypes Type;

    Boolean HasRobber;

    private readonly UInt32 productionNumber;
    #endregion

    #region Construction
    public ResourceProvider(ResourceTypes type, UInt32 productionNumber)
    {
      this.Type = type;

      if (productionNumber < 2 || productionNumber > 12)
      {
        throw new ArgumentOutOfRangeException("Parameter 'productionNumber' must be within range 2..12", (Exception)null);
      }

      this.productionNumber = productionNumber;
    }

    public ResourceProvider()
    {
      this.Type = ResourceTypes.None;
    }
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

      return (first.Type == second.Type && first.productionNumber == second.productionNumber);
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

      return this.Type == other.Type && this.productionNumber == other.productionNumber;
    }

    public override Int32 GetHashCode()
    {
      var factor = 0;
      switch (this.Type)
      {
        case ResourceTypes.Brick: factor = 1; break;
        case ResourceTypes.Grain: factor = 10; break;
        case ResourceTypes.Lumber: factor = 100; break;
        case ResourceTypes.None: factor = 1000; break;
        case ResourceTypes.Ore: factor = 10000; break;
        case ResourceTypes.Wool: factor = 100000; break;
      }

      return (Int32)(this.productionNumber * factor); 
    }
    
    public Boolean ProducesResources(UInt32 rolledNumber) { throw new NotImplementedException(); }
    #endregion
  }
}
