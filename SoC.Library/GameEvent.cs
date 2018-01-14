
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class GameEvent
  {
    #region Fields
    public readonly Guid PlayerId;
    #endregion

    #region Construction
    public GameEvent(Guid playerId)
    {
      this.PlayerId = playerId;
    }
    #endregion

    #region Methods
    public override Boolean Equals(Object obj)
    {
      if (obj == null)
      {
        return false;
      }

      return this.PlayerId == ((GameEvent)obj).PlayerId;
    }

    public override Int32 GetHashCode()
    {
      return base.GetHashCode();
    }
    #endregion
  }

  public class BuyDevelopmentCardEvent : GameEvent
  {
    public BuyDevelopmentCardEvent(Guid playerId) : base(playerId) { }
  }

  public class PlayKnightCardEvent : GameEvent
  {
    public PlayKnightCardEvent(Guid playerId) : base(playerId) { }
  }

  public class PlayMonopolyCardEvent : GameEvent
  {
    public readonly Dictionary<Guid, ResourceClutch> Resources = new Dictionary<Guid, ResourceClutch>();

    public PlayMonopolyCardEvent(Guid playerId, Dictionary<Guid, ResourceClutch> resources) : base(playerId)
    {
      this.Resources = resources;
    }
  }

  public class ResourceLostEvent : GameEvent
  {
    #region Fields
    public readonly ResourceClutch Resources;
    #endregion

    public ResourceLostEvent(Guid playerId, ResourceClutch resources) : base(playerId)
    {

    }
  }
}
