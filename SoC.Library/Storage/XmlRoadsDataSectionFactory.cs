
namespace Jabberwocky.SoC.Library.Storage
{
  using System.Xml;

  public class XmlRoadsDataSectionFactory
  {
    public static XmlGameDataSection[] CreateSectionArray(XmlDocument doc)
    {
      var roadsNodes = doc.SelectNodes("/game/roads/road");
      var sections = new XmlGameDataSection[roadsNodes.Count];

      var builder = new XmlRoadDataSectionFactory();

      for (var index = 0; index < roadsNodes.Count; index++)
      {
        builder.SetValues(roadsNodes[index]);
        sections[index] = new XmlGameDataSection(builder);
      }

      return sections;
    }
  }
}
