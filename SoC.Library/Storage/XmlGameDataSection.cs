
namespace Jabberwocky.SoC.Library.Storage
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  public class XmlGameDataSection : IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
    private readonly Dictionary<GameDataValueKeys, Boolean> booleanValues;
    private readonly Dictionary<GameDataValueKeys, Guid> identityValues;
    private readonly Dictionary<GameDataValueKeys, Int32[]> integerArrayValues;
    private readonly Dictionary<GameDataValueKeys, Int32> integerValues;
    private readonly Dictionary<GameDataSectionKeys, XmlGameDataSection> sections;
    private readonly Dictionary<GameDataSectionKeys, XmlGameDataSection[]> sectionArrays;
    private readonly Dictionary<GameDataValueKeys, String> stringValues;

    public XmlGameDataSection(XmlGameDataSectionBaseFactory factory)
    {
      this.booleanValues = factory.GetBooleans();
      this.identityValues = factory.GetIdentities();
      this.integerValues = factory.GetIntegers();
      this.integerArrayValues = factory.GetIntegerArrays();
      this.sections = factory.GetSections();
      this.sectionArrays = factory.GetSectionArrays();
      this.stringValues = factory.GetStrings();
    }

    public bool GetBooleanValue(GameDataValueKeys key)
    {
      throw new NotImplementedException();
    }

    public ResourceTypes[] GetEnumArrayValue(GameDataValueKeys key)
    {
      throw new NotImplementedException();
    }

    public Guid GetIdentityValue(GameDataValueKeys key)
    {
      return this.identityValues[key];
    }

    public int[] GetIntegerArrayValue(GameDataValueKeys key)
    {
      return this.integerArrayValues[key];
    }

    public int GetIntegerValue(GameDataValueKeys key)
    {
      return this.integerValues[key];
    }

    public IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> this[GameDataSectionKeys sectionKey]
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>[] GetSections(GameDataSectionKeys sectionKey)
    {
      return this.sectionArrays[sectionKey];
    }

    public string GetStringValue(GameDataValueKeys key)
    {
      return this.stringValues[key];
    }

    public string GetStringValues(GameDataValueKeys key)
    {
      throw new NotImplementedException();
    }
  }

  public class XmlGameDataSectionBaseFactory
  {
    public virtual Dictionary<GameDataValueKeys, bool> GetBooleans()
    {
      return null;
    }

    public virtual Dictionary<GameDataValueKeys, int> GetIntegers()
    {
      return null;
    }

    public virtual Dictionary<GameDataValueKeys, int[]> GetIntegerArrays()
    {
      return null;
    }

    public virtual Dictionary<GameDataSectionKeys, XmlGameDataSection> GetSections()
    {
      return null;
    }

    public virtual Dictionary<GameDataValueKeys, String> GetStrings()
    {
      return null;
    }

    public virtual Dictionary<GameDataValueKeys, Guid> GetIdentities()
    {
      return null;
    }

    public virtual Dictionary<GameDataSectionKeys, XmlGameDataSection[]> GetSectionArrays()
    {
      return null;
    }
  }

  public class XmlGameBoardDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, String> stringValues;
    private readonly Dictionary<GameDataValueKeys, Int32[]> integerArrayValues;
    private readonly Dictionary<GameDataSectionKeys, XmlGameDataSection[]> sectionArraysValues;

    public XmlGameBoardDataSectionFactory(XmlDocument document)
    {
      var root = document.DocumentElement;
      var node = root.SelectSingleNode("/game/board/hexes/resources");
      this.stringValues = new Dictionary<GameDataValueKeys, string> { { GameDataValueKeys.HexResources, node.InnerText } };

      node = root.SelectSingleNode("/game/board/hexes/production");
      var rawValues = node.InnerText.Split(',');
      if (rawValues.Length > 0)
      {
        var values = new int[rawValues.Length];

        for (var index = 0; index < values.Length; index++)
        {
          values[index] = Int32.Parse(rawValues[index]);
        }

        this.integerArrayValues = new Dictionary<GameDataValueKeys, int[]> { { GameDataValueKeys.HexProduction, values } };
      }
    }

    public override Dictionary<GameDataValueKeys, int[]> GetIntegerArrays()
    {
      return this.integerArrayValues;
    }

    public override Dictionary<GameDataValueKeys, string> GetStrings()
    {
      return this.stringValues;
    }
  }

  public class XmlGamePlayerDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, Guid> identityValues;
    private readonly Dictionary<GameDataValueKeys, Int32> integerValues;
    private readonly Dictionary<GameDataValueKeys, String> stringValues;

    public XmlGamePlayerDataSectionFactory(XmlNode playerNode)
    {
      var id = new Guid(playerNode.Attributes["id"].Value);
      this.identityValues = new Dictionary<GameDataValueKeys, Guid> { { GameDataValueKeys.PlayerId, id } };

      this.integerValues = new Dictionary<GameDataValueKeys, Int32> {
        { GameDataValueKeys.PlayerBrick, Int32.Parse(playerNode.Attributes["brick"].Value)  },
        { GameDataValueKeys.PlayerGrain, Int32.Parse(playerNode.Attributes["grain"].Value)  },
        { GameDataValueKeys.PlayerLumber, Int32.Parse(playerNode.Attributes["lumber"].Value)  },
        { GameDataValueKeys.PlayerOre, Int32.Parse(playerNode.Attributes["ore"].Value)  },
        { GameDataValueKeys.PlayerWool, Int32.Parse(playerNode.Attributes["wool"].Value)  }
      };

      this.stringValues = new Dictionary<GameDataValueKeys, String> { { GameDataValueKeys.PlayerName, playerNode.Attributes["name"].Value } };
    }

    public override Dictionary<GameDataValueKeys, Guid> GetIdentities()
    {
      return this.identityValues;
    }

    public override Dictionary<GameDataValueKeys, int> GetIntegers()
    {
      return this.integerValues;
    }

    public override Dictionary<GameDataValueKeys, string> GetStrings()
    {
      return this.stringValues;
    }
  }

  public class XmlBuildingsDataSectionFactory
  {
    public static XmlGameDataSection[] CreateSectionArray(XmlDocument doc)
    {
      var settlementNodes = doc.SelectNodes("/game/settlements/settlement");
      var buildingSections = new XmlGameDataSection[settlementNodes.Count];

      var builder = new XmlBuildingDataSectionFactory();

      for (var index = 0; index < settlementNodes.Count; index++)
      {
        builder.SetValues(settlementNodes[index]);
        buildingSections[index] = new XmlGameDataSection(builder);
      }

      return buildingSections;
    }
  }

  public class XmlBuildingDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, Guid> identityValues = new Dictionary<GameDataValueKeys, Guid>();
    private readonly Dictionary<GameDataValueKeys, Int32> integerValues = new Dictionary<GameDataValueKeys, Int32>();

    public void SetValues(XmlNode buildingNode)
    {
      this.identityValues.Clear();
      this.integerValues.Clear();

      this.identityValues.Add(GameDataValueKeys.SettlementOwner, Guid.Parse(buildingNode.Attributes["playerid"].Value));
      this.integerValues.Add(GameDataValueKeys.SettlementLocation, Int32.Parse(buildingNode.Attributes["location"].Value));
    }

    public override Dictionary<GameDataValueKeys, Guid> GetIdentities()
    {
      return this.identityValues;
    }

    public override Dictionary<GameDataValueKeys, int> GetIntegers()
    {
      return this.integerValues;
    }
  }

  public class XmlRoadsDataSectionFactory
  {
    public static XmlGameDataSection[] CreateSectionArray(XmlDocument doc)
    {
      var roadsNodes = doc.SelectNodes("/game/roads/road");
      var sections = new XmlGameDataSection[roadsNodes.Count];

      var builder = new XmlRoadDataSectionFactory();

      for (var index = 0; index < roadsNodes.Count; index++)
      {
        builder.SetValues(roadsNodes[index]);
        sections[index++] = new XmlGameDataSection(builder);
      }

      return sections;
    }
  }

  public class XmlRoadDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, Guid> identityValues = new Dictionary<GameDataValueKeys, Guid>();
    private readonly Dictionary<GameDataValueKeys, Int32> integerValues = new Dictionary<GameDataValueKeys, Int32>();

    public void SetValues(XmlNode roadNode)
    {
      this.identityValues.Clear();
      this.integerValues.Clear();

      this.identityValues.Add(GameDataValueKeys.RoadOwner, Guid.Parse(roadNode.Attributes["playerid"].Value));
      this.integerValues.Add(GameDataValueKeys.RoadStart, Int32.Parse(roadNode.Attributes["start"].Value));
      this.integerValues.Add(GameDataValueKeys.RoadEnd, Int32.Parse(roadNode.Attributes["end"].Value));
    }

    public override Dictionary<GameDataValueKeys, Guid> GetIdentities()
    {
      return this.identityValues;
    }

    public override Dictionary<GameDataValueKeys, int> GetIntegers()
    {
      return this.integerValues;
    }
  }
}
