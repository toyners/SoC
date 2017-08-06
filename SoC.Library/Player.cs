
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

    public Player(String name) : this()
    {
      this.Name = name;
    }

    public Guid Id { get; private set; }

    public String Name { get; private set; }

    public PlayerDataView GetDataView()
    {
      var dataView = new PlayerDataView();

      dataView.Id = this.Id;
      dataView.Name = this.Name;
      dataView.ResourceCards = 0u;
      dataView.HiddenDevelopmentCards = 0;
      dataView.DisplayedDevelopmentCards = null;

      return dataView;
    }
  }
}
