
namespace Jabberwocky.SoC.Library.Storage
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  public class XmlBuildingDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private Dictionary<GameDataValueKeys, Guid> identityValues;
    private Dictionary<GameDataValueKeys, Int32> integerValues;

    public void SetValues(XmlNode buildingNode)
    {
      this.identityValues = new Dictionary<GameDataValueKeys, Guid>();
      this.integerValues = new Dictionary<GameDataValueKeys, Int32>();

      this.identityValues.Add(GameDataValueKeys.SettlementOwner, Guid.Parse(buildingNode.Attributes["playerid"].Value));
      this.integerValues.Add(GameDataValueKeys.SettlementLocation, Int32.Parse(buildingNode.Attributes["location"].Value));
    }

    public override Dictionary<GameDataValueKeys, Guid> GetIdentities()
    {
      return this.identityValues;
    }

    public override Dictionary<GameDataValueKeys, int> GetIntegers()
    {
      return this.integerValues;
    }
  }
}
