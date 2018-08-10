
namespace Jabberwocky.SoC.Library.Storage
{
  using System.IO;
  using System.Xml;

  public class XmlGameDataReader : IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
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
        return new XmlGameDataSection(null);
      }
    }
  }
}
