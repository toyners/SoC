
namespace Jabberwocky.SoC.Library.Store
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  public class XmlGamePlayerDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, Guid> identityValues;
    private readonly Dictionary<GameDataValueKeys, Int32> integerValues;
    private readonly Dictionary<GameDataValueKeys, String> stringValues;

    public XmlGamePlayerDataSectionFactory(XmlNode playerNode)
    {
      var id = new Guid(playerNode.Attributes["id"].Value);
      this.identityValues = new Dictionary<GameDataValueKeys, Guid> { { GameDataValueKeys.PlayerId, id } };

      this.integerValues = new Dictionary<GameDataValueKeys, Int32> {
        { GameDataValueKeys.PlayerBrick, Int32.Parse(playerNode.Attributes["brick"].Value)  },
        { GameDataValueKeys.PlayerGrain, Int32.Parse(playerNode.Attributes["grain"].Value)  },
        { GameDataValueKeys.PlayerLumber, Int32.Parse(playerNode.Attributes["lumber"].Value)  },
        { GameDataValueKeys.PlayerOre, Int32.Parse(playerNode.Attributes["ore"].Value)  },
        { GameDataValueKeys.PlayerWool, Int32.Parse(playerNode.Attributes["wool"].Value)  }
      };

      this.stringValues = new Dictionary<GameDataValueKeys, String> { { GameDataValueKeys.PlayerName, playerNode.Attributes["name"].Value } };
    }

    public override Dictionary<GameDataValueKeys, Guid> GetIdentities()
    {
      return this.identityValues;
    }

    public override Dictionary<GameDataValueKeys, int> GetIntegers()
    {
      return this.integerValues;
    }

    public override Dictionary<GameDataValueKeys, string> GetStrings()
    {
      return this.stringValues;
    }
  }
}
