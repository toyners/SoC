
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

  public class PlayerWithLargestArmyChangedEvent : GameEvent
  {
    public readonly Guid PreviousPlayerWithLargestArmyId;

    public PlayerWithLargestArmyChangedEvent(Guid playerId, Guid previousPlayerWithLargestArmyId) : base(playerId)
    {
      this.PreviousPlayerWithLargestArmyId = previousPlayerWithLargestArmyId;
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      return this.PreviousPlayerWithLargestArmyId == ((PlayerWithLargestArmyChangedEvent)obj).PreviousPlayerWithLargestArmyId;
    }

    public override Int32 GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
