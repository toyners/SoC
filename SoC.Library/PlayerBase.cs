
namespace Jabberwocky.SoC.Library
{
  using System;

  public class PlayerBase
  {
    public readonly Guid Id;
    public DevelopmentCard[] DisplayedDevelopmentCards;

    public PlayerBase()
    {
      this.Id = Guid.NewGuid();
    }
  }
}
