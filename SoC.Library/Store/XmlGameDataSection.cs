
namespace Jabberwocky.SoC.Library.Store
{
  using System;
  using System.Collections.Generic;

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
}
