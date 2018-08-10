
namespace Jabberwocky.SoC.Library.Storage
{
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;

  public class XmlGameDataReader : IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
    private Dictionary<GameDataSectionKeys, XmlGameDataSection> sections;

    public XmlGameDataReader(MemoryStream stream)
    {
    }

    public XmlGameDataReader(XmlReader stream)
    {
    }

    public IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> this[GameDataSectionKeys sectionKey]
    {
      get
      {
        if (!this.sections.TryGetValue(sectionKey, out var section))
        {
          throw new KeyNotFoundException($"{sectionKey} not found in game data");
        }

        return section;
      }
    }
  }
}
