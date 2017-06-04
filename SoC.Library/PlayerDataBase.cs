
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class PlayerDataBase
  {
    public Guid Id { get; protected set; }
    public DevelopmentCard[] DisplayedDevelopmentCards;

    public PlayerDataBase()
    {
      this.Id = Guid.NewGuid();
    }
  }

  public class Player : IPlayer
  {
    public Player()
    {
      this.Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
  }
}
