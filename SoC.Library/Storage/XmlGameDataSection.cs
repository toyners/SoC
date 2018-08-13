
namespace Jabberwocky.SoC.Library.Storage
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;

  public class XmlGameDataSection : IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
    private readonly Dictionary<GameDataSectionKeys, XmlGameDataSection> sections;
    private readonly Dictionary<GameDataValueKeys, Int32> integerValues;
    private readonly Dictionary<GameDataValueKeys, Boolean> booleanValues;

    public XmlGameDataSection(XmlGameDataSectionBaseFactory factory)
    {
      this.sections = factory.GetSections();
      this.integerValues = factory.GetIntegers();
      this.booleanValues = factory.GetBooleans();
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
      throw new NotImplementedException();
    }

    public int GetIntegerValue(GameDataValueKeys key)
    {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
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

    public virtual Dictionary<GameDataSectionKeys, XmlGameDataSection> GetSections()
    {
      return null;
    }
  }

  public class XmlGameBoardDataSection : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, String> stringValues;
    private readonly Dictionary<GameDataValueKeys, Int32[]> integerArrayValues;

    public XmlGameBoardDataSection(XmlReader reader)
    {
      var startingName = reader.Name;
      while (!reader.EOF && reader.Name != startingName && reader.NodeType != XmlNodeType.EndElement)
      {
        if (reader.Name == "resources" && reader.NodeType == XmlNodeType.Element)
        {
          this.stringValues = new Dictionary<GameDataValueKeys, string> { { GameDataValueKeys.HexResources, reader.ReadElementContentAsString() } };
          continue;
        }

        if (reader.Name == "production" && reader.NodeType == XmlNodeType.Element)
        {
          var production = reader.ReadElementContentAsString();
          var values = production.Split(',');
          continue;
        }

        reader.Read();
      }
    }
  }
}
