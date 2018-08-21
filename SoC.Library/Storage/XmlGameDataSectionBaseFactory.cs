
namespace Jabberwocky.SoC.Library.Storage
{
  using System;
  using System.Collections.Generic;

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
}
