
namespace Jabberwocky.SoC.Library.GameActions
{
  using System;
  using Interfaces;

  public class TradeWithBankAction : PlayerAction
  {
    public readonly ResourceTypes GivingType;
    public readonly ResourceTypes ReceivingType;
    public readonly Int32 ReceivingCount;

    public TradeWithBankAction(ResourceTypes givingType, ResourceTypes receivingType, Int32 receivingCount) : base(PlayerActionTypes.TradeWithBank)
    {
      this.GivingType = givingType;
      this.ReceivingType = receivingType;
      this.ReceivingCount = receivingCount;
    }
  }
}
