
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class Player : IPlayer
  {
    public Player()
    {
      this.Id = Guid.NewGuid();
    }

    public PlayerData Data { get; private set; }

    public Guid Id { get; private set; }
  }
}
