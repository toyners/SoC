
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class Player : IPlayer
  {
    #region Construction
    public Player()
    {
      this.Id = Guid.NewGuid();
    }

    public Player(String name) : this()
    {
      this.Name = name;
    }
    #endregion

    #region Properties
    public Int32 BrickCount { get; protected set; }

    public Int32 GrainCount { get; protected set; }
    
    public Guid Id { get; private set; }

    public Int32 LumberCount { get; protected set; }
    
    public String Name { get; private set; }

    public Int32 OreCount { get; protected set; }
    
    public virtual Int32 ResourcesCount
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Int32 WoolCount { get; protected set; }
    #endregion

    #region Methods
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

    public void RemoveResources(ResourceClutch resourceClutch)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
