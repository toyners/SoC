
namespace Jabberwocky.SoC.Library.Storage
{
  using System.Xml;

  public class XmlBuildingsDataSectionFactory
  {
    public static XmlGameDataSection[] CreateSectionArray(XmlDocument doc)
    {
      var settlementNodes = doc.SelectNodes("/game/settlements/settlement");
      var buildingSections = new XmlGameDataSection[settlementNodes.Count];

      var builder = new XmlBuildingDataSectionFactory();

      for (var index = 0; index < settlementNodes.Count; index++)
      {
        builder.SetValues(settlementNodes[index]);
        buildingSections[index] = new XmlGameDataSection(builder);
      }

      return buildingSections;
    }
  }
}
