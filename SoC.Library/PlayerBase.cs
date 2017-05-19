
namespace Jabberwocky.SoC.Library
{
  using System;

  public class PlayerBase
  {
    public readonly Guid PlayerId;
    DevelopmentCard[] DisplayedDevelopmentCards;

    public PlayerBase()
    {
      this.PlayerId = Guid.NewGuid();
    }
  }
}
