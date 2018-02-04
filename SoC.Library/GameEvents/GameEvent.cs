
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

      if (this.GetType() != obj.GetType())
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

  public class ResourceTransactionEvent : GameEvent
  {
    #region Fields
    public readonly ResourceTransactionList ResourceTransactions;
    #endregion

    public ResourceTransactionEvent(Guid playerId, ResourceTransactionList resourceTransactions) : base(playerId)
    {
      this.ResourceTransactions = resourceTransactions;
    }

    public ResourceTransactionEvent(Guid playerId, ResourceTransaction resourceTransaction) : base(playerId)
    {
      this.ResourceTransactions = new ResourceTransactionList();
      this.ResourceTransactions.Add(resourceTransaction);
    }
  }

  public class TradeWithBankEvent : GameEvent
  {
    public readonly ResourceTransaction PaymentResourceTransaction;
    public readonly ResourceTransaction ReceivingResourceTransaction;

    public TradeWithBankEvent(Guid playerId, Guid bankId, ResourceClutch givingResources, ResourceClutch receivingResources) : base(playerId)
    {
      this.PaymentResourceTransaction = new ResourceTransaction(bankId, playerId, givingResources);
      this.ReceivingResourceTransaction = new ResourceTransaction(playerId, bankId, receivingResources);
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      var other = (TradeWithBankEvent)obj;
      return this.PaymentResourceTransaction.Equals(other.PaymentResourceTransaction) &&
        this.ReceivingResourceTransaction.Equals(other.ReceivingResourceTransaction);
    }
  }

  public class RoadSegmentBuiltEvent : GameEvent
  {
    public readonly UInt32 StartLocation;
    public readonly UInt32 EndLocation;

    public RoadSegmentBuiltEvent(Guid playerId, UInt32 startLocation, UInt32 endLocation) : base(playerId)
    {
      this.StartLocation = startLocation;
      this.EndLocation = endLocation;
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      var other = (RoadSegmentBuiltEvent)obj;
      return this.StartLocation == other.StartLocation && this.EndLocation == other.EndLocation;
    }
  }
}
