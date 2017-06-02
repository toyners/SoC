
namespace Jabberwocky.SoC.Library
{
  using System;

  public class PlayerBase
  {
    public Guid Id { get; protected set; }
    public DevelopmentCard[] DisplayedDevelopmentCards;

    public PlayerBase()
    {
      this.Id = Guid.NewGuid();
    }
  }
}
