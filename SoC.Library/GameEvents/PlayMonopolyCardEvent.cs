
namespace Jabberwocky.SoC.Library
{
  using System;
  using GameEvents;

  public class PlayMonopolyCardEvent : GameEvent
  {
    public readonly ResourceTransactionList ResourceTransactionList;

    public PlayMonopolyCardEvent(Guid playerId, ResourceTransactionList resourceTransactionList) : base(playerId)
    {
      this.ResourceTransactionList = resourceTransactionList;
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      var otherList = this.GetOtherTransactionList(obj);

      return this.ResourceTransactionListEquals(otherList);
    }

    protected virtual ResourceTransactionList GetOtherTransactionList(Object obj)
    {
      return ((PlayMonopolyCardEvent)obj).ResourceTransactionList;
    }

    private Boolean ResourceTransactionListEquals(ResourceTransactionList otherList)
    {
      if (this.ResourceTransactionList == null && otherList == null)
      {
        return true;
      }

      if (this.ResourceTransactionList == null || otherList == null)
      {
        return false;
      }

      if (this.ResourceTransactionList.Count != otherList.Count)
      {
        return false;
      }

      for (var i = 0; i < this.ResourceTransactionList.Count; i++)
      {
        var left = this.ResourceTransactionList[i];
        var right = otherList[i];
        if (left.GivingPlayerId != right.GivingPlayerId ||
            left.ReceivingPlayerId != right.ReceivingPlayerId ||
            left.Resources != right.Resources)
        {
          return false;
        }
      }

      return true;
    }
  }
}
