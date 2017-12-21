
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  public interface IDevelopmentCardHolder
  {
    Boolean HasCards { get; }

    Boolean TryGetNextCard(out DevelopmentCard card);
  }
}
