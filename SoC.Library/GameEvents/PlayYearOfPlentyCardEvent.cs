
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

  public class PlayYearOfPlentyCardEvent : PlayMonopolyCardEvent
  {
    public PlayYearOfPlentyCardEvent(Guid playerId, ResourceTransactionList resourceTransactionList) : base(playerId, resourceTransactionList) {}

    protected override ResourceTransactionList GetOtherTransactionList(Object obj)
    {
      return ((PlayYearOfPlentyCardEvent)obj).ResourceTransactionList;
    }
  }
}
