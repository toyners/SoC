
namespace Jabberwocky.SoC.Library
{
  using System;
  using GameEvents;

  public class PlayerWithLargestArmyChangedEvent : GameEvent
  {
    #region Fields
    public readonly Guid PreviousPlayerWithLargestArmyId;
    #endregion

    #region Construction
    public PlayerWithLargestArmyChangedEvent(Guid playerId, Guid previousPlayerWithLargestArmyId) : base(playerId)
    {
      this.PreviousPlayerWithLargestArmyId = previousPlayerWithLargestArmyId;
    }
    #endregion

    #region Methods
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
    #endregion
  }
}
