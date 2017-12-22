
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class DevelopmentCardHolder : IDevelopmentCardHolder
  {
    public Boolean HasCards
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Boolean TryGetNextCard(out DevelopmentCard card)
    {
      throw new NotImplementedException();
    }
  }
}
