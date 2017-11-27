﻿
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Xml;
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

    public virtual Boolean IsComputer { get { return false; } }

    public Int32 LumberCount { get; protected set; }

    public String Name { get; private set; }

    public Int32 OreCount { get; protected set; }

    public virtual Int32 ResourcesCount
    {
      get
      {
        return this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount;
      }
    }

    public Int32 WoolCount { get; protected set; }

    public UInt32 VictoryPoints { get; protected set; }
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
      dataView.ResourceCards = this.ResourcesCount;
      dataView.HiddenDevelopmentCards = 0;
      dataView.DisplayedDevelopmentCards = null;
      dataView.IsComputer = this.IsComputer;

      return dataView;
    }

    /// <summary>
    /// Loads player properties from stream. Stream must contain player id and name.
    /// </summary>
    /// <param name="stream">Stream containing player properties.</param>

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

    public void RemoveResourcesForRoad()
    {
      this.BrickCount--;
      this.LumberCount--;
    }

    internal void Load(XmlReader reader)
    {
      this.Id = Guid.Empty;

      try
      {
        var idValue = reader.GetAttribute("id");
        if (!String.IsNullOrEmpty(idValue))
        {
          this.Id = Guid.Parse(idValue);
        }

        this.Name = reader.GetAttribute("name");
        this.BrickCount = this.GetValueOrZero(reader, "brick");
        this.GrainCount = this.GetValueOrZero(reader, "grain");
        this.LumberCount = this.GetValueOrZero(reader, "lumber");
        this.OreCount = this.GetValueOrZero(reader, "ore");
        this.WoolCount = this.GetValueOrZero(reader, "wool");
      }
      catch (Exception e)
      {
        throw new Exception("Ëxception thrown during player loading.", e);
      }

      if (this.Id == Guid.Empty)
      {
        throw new Exception("No id found for player in stream.");
      }

      if (String.IsNullOrEmpty(this.Name))
      {
        throw new Exception("No name found for player in stream.");
      }
    }

    private Int32 GetValueOrZero(XmlReader reader, String attributeName)
    {
      var value = reader.GetAttribute(attributeName);
      return !String.IsNullOrEmpty(value) ? Int32.Parse(value) : 0;
    }
    #endregion
  }
}
