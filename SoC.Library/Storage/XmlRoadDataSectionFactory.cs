
namespace Jabberwocky.SoC.Library.Storage
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  public class XmlRoadDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private Dictionary<GameDataValueKeys, Guid> identityValues;
    private Dictionary<GameDataValueKeys, Int32> integerValues;

    public void SetValues(XmlNode roadNode)
    {
      this.identityValues = new Dictionary<GameDataValueKeys, Guid>();
      this.integerValues = new Dictionary<GameDataValueKeys, Int32>();

      this.identityValues.Add(GameDataValueKeys.RoadOwner, Guid.Parse(roadNode.Attributes["playerid"].Value));
      this.integerValues.Add(GameDataValueKeys.RoadStart, Int32.Parse(roadNode.Attributes["start"].Value));
      this.integerValues.Add(GameDataValueKeys.RoadEnd, Int32.Parse(roadNode.Attributes["end"].Value));
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
