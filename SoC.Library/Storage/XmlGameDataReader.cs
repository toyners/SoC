
namespace Jabberwocky.SoC.Library.Storage
{
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;
  using System.Xml.Linq;

  public class XmlGameDataReader : IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
    private Dictionary<GameDataSectionKeys, XmlGameDataSection> sections;

    public XmlGameDataReader(MemoryStream stream)
    {
      this.sections = new Dictionary<GameDataSectionKeys, XmlGameDataSection>();

      using (var reader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true }))
      {
        while (!reader.EOF)
        {
          if (reader.Name == "resources" && reader.NodeType == XmlNodeType.Element)
          {
            if (!this.sections.TryGetValue(GameDataSectionKeys.GameBoard, out var section))
            {
              section = new XmlGameDataSection(null);
              this.sections.Add(GameDataSectionKeys.GameBoard, section);
            }

            reader.ReadElementContentAsString();
          }

          if (reader.Name == "production" && reader.NodeType == XmlNodeType.Element)
          {

          }
        }
      }
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
