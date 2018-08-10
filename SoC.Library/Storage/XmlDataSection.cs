
namespace Jabberwocky.SoC.Library.Storage
{
  using System;

  public class XmlDataSection : IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
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

    public IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> GetSection(GameDataSectionKeys sectionKey)
    {
      throw new NotImplementedException();
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
}
