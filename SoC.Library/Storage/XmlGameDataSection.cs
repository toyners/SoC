
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
    private readonly Dictionary<GameDataValueKeys, String> stringValues;

    public XmlGameDataSection(XmlGameDataSectionBaseFactory factory)
    {
      this.booleanValues = factory.GetBooleans();
      this.identityValues = factory.GetIdentities();
      this.integerValues = factory.GetIntegers();
      this.integerArrayValues = factory.GetIntegerArrays();
      this.sections = factory.GetSections();
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
      throw new NotImplementedException();
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
      throw new NotImplementedException();
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
  }

  public class XmlGameBoardDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, String> stringValues;
    private readonly Dictionary<GameDataValueKeys, Int32[]> integerArrayValues;

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
        { GameDataValueKeys.PlayerBrick, Int32.Parse(playerNode["brick"].Value)  },
        { GameDataValueKeys.PlayerGrain, Int32.Parse(playerNode["grain"].Value)  },
        { GameDataValueKeys.PlayerLumber, Int32.Parse(playerNode["lumber"].Value)  },
        { GameDataValueKeys.PlayerOre, Int32.Parse(playerNode["ore"].Value)  },
        { GameDataValueKeys.PlayerWool, Int32.Parse(playerNode["wool"].Value)  }
      };

      this.stringValues = new Dictionary<GameDataValueKeys, String> { { GameDataValueKeys.PlayerName, playerNode["name"].Value } };
    }
  }
}
