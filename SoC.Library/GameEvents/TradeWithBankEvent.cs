
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

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
}
