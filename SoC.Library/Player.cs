
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.IO;
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

    /// <summary>
    /// Loads player properties from stream. Original player id is preserved.
    /// </summary>
    /// <param name="stream">Stream containing player properties.</param>
    public void Load(Stream stream)
    {
      this.Id = Guid.Empty;
      try
      {
        using (var reader = XmlReader.Create(stream))
        {
          reader.Read(); // Move to first node
          if (reader.Name == "player" && reader.NodeType == XmlNodeType.Element)
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
          else
          {
            throw new XmlException("Element is '" + reader.Name + "' but should be 'plaýer'.");
          }
        }
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
      return value != null ? Int32.Parse(value) : 0;
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
