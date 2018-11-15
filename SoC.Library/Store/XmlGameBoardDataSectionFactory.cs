
namespace Jabberwocky.SoC.Library.Store
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  public class XmlGameBoardDataSectionFactory : XmlGameDataSectionBaseFactory
  {
    private readonly Dictionary<GameDataValueKeys, String> stringValues;
    private readonly Dictionary<GameDataValueKeys, Int32[]> integerArrayValues;
    private readonly Dictionary<GameDataSectionKeys, XmlGameDataSection[]> sectionArraysValues;

    public XmlGameBoardDataSectionFactory(XmlDocument document)
    {
      var root = document.DocumentElement;
      var node = root.SelectSingleNode("/game/board/hexes/resources");
      this.stringValues = new Dictionary<GameDataValueKeys, string> { { GameDataValueKeys.HexResources, node.InnerText } };

      node = root.SelectSingleNode("/game/board/hexes/production");
      var rawValues = node.InnerText.Split(',');
      if (rawValues.Length > 0)
      {
        var values = new int[rawValues.Length];

        for (var index = 0; index < values.Length; index++)
        {
          values[index] = Int32.Parse(rawValues[index]);
        }

        this.integerArrayValues = new Dictionary<GameDataValueKeys, int[]> { { GameDataValueKeys.HexProduction, values } };
      }
    }

    public override Dictionary<GameDataValueKeys, int[]> GetIntegerArrays()
    {
      return this.integerArrayValues;
    }

    public override Dictionary<GameDataValueKeys, string> GetStrings()
    {
      return this.stringValues;
    }
  }
}
