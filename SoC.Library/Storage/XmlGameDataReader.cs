
namespace Jabberwocky.SoC.Library.Storage
{
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;

  public class XmlGameDataReader : IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
    private Dictionary<GameDataSectionKeys, XmlGameDataSection> sections;

    public XmlGameDataReader(Stream stream)
    {
      var sr = new StreamReader(stream);
      var content = sr.ReadToEnd();
      var doc = new XmlDocument();
      doc.LoadXml(content);

      this.sections = new Dictionary<GameDataSectionKeys, XmlGameDataSection>();
      var section = new XmlGameDataSection(new XmlGameBoardDataSectionFactory(doc));
      this.sections.Add(GameDataSectionKeys.GameBoard, section);
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
