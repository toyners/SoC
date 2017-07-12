
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class Player : IPlayer
  {
    public Player()
    {
      this.Data = new PlayerData();
      this.Data.Id = Guid.NewGuid();
    }

    public PlayerData Data { get; private set; }

    public Guid Id { get { return this.Data.Id; } }

    public PlayerDataBase GetDataView()
    {
      return this.Data;
    }
  }
}
