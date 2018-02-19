
namespace Jabberwocky.SoC.Library.GameActions
{
  using System;
  using Enums;

  public class TradeWithBankAction : ComputerPlayerAction
  {
    public readonly ResourceTypes GivingType;
    public readonly ResourceTypes ReceivingType;
    public readonly Int32 ReceivingCount;

    public TradeWithBankAction(ResourceTypes givingType, ResourceTypes receivingType, Int32 receivingCount) : base(ComputerPlayerActionTypes.TradeWithBank)
    {
      this.GivingType = givingType;
      this.ReceivingType = receivingType;
      this.ReceivingCount = receivingCount;
    }
  }
}
