
namespace Jabberwocky.SoC.Library.Storage
{
  using System;
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

      var playerNode = doc.DocumentElement.SelectSingleNode("/game/players/playerOne");
      section = new XmlGameDataSection(new XmlGamePlayerDataSectionFactory(playerNode));
      this.sections.Add(GameDataSectionKeys.PlayerOne, section);

      foreach (var token in new List<Tuple<String, GameDataSectionKeys>> {
        new Tuple<string, GameDataSectionKeys>("playerTwo", GameDataSectionKeys.PlayerTwo),
        new Tuple<string, GameDataSectionKeys>("playerThree", GameDataSectionKeys.PlayerThree),
        new Tuple<string, GameDataSectionKeys>("playerFour", GameDataSectionKeys.PlayerFour)})
      {
        playerNode = doc.DocumentElement.SelectSingleNode("/game/players/" + token.Item1);
        if (playerNode == null)
        {
          continue;
        }

        section = new XmlGameDataSection(new XmlGamePlayerDataSectionFactory(playerNode));
        this.sections.Add(token.Item2, section);
      }

      section = new XmlGameDataSection(new XmlBuildingDataSectionFactory(doc));
      this.sections.Add(GameDataSectionKeys.Buildings, section);
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
