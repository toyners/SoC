
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
    public void AddResources(ResourceClutch resourceClutch)
    {
      this.BrickCount += resourceClutch.BrickCount;
      this.GrainCount += resourceClutch.GrainCount;
      this.LumberCount += resourceClutch.LumberCount;
      this.OreCount += resourceClutch.OreCount;
      this.WoolCount += resourceClutch.WoolCount;
    }

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
      if (this.BrickCount - resourceClutch.BrickCount < 0 ||
          this.GrainCount - resourceClutch.GrainCount < 0 ||
          this.LumberCount - resourceClutch.LumberCount < 0 ||
          this.OreCount - resourceClutch.OreCount < 0 ||
          this.WoolCount - resourceClutch.WoolCount < 0)
      {
        throw new ArithmeticException("No resource count can be negative.");
      }

      this.BrickCount -= resourceClutch.BrickCount;
      this.GrainCount -= resourceClutch.GrainCount;
      this.LumberCount -= resourceClutch.LumberCount;
      this.OreCount -= resourceClutch.OreCount;
      this.WoolCount -= resourceClutch.WoolCount;
    }
    #endregion
  }
}
