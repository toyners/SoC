
namespace Jabberwocky.SoC.Library.UnitTests.Storage_Tests
{
  using System.IO;
  using System.Text;
  using Jabberwocky.SoC.Library.Storage;
  using NUnit.Framework;

  [TestFixture]
  public class XmlGameDataReader_UnitTests
  {
    [Test]
    [Category("XmlGameDataReader")]
    public void Load_HexDataOnly_ResourceProvidersLoadedCorrectly()
    {
      // Arrange
      var content = "<game><board><hexes>" +
        "<resources>glbglogob gwwwlwlbo</resources>" +
        "<production>9,8,5,12,11,3,6,10,6,0,4,11,2,4,3,5,9,10,8</production>" +
        "</hexes></board></game>";

      XmlGameDataReader xmlGameDataReader;
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      using (var stream = new MemoryStream(contentBytes))
      {
        xmlGameDataReader = new XmlGameDataReader(stream);
      }

      // Assert
      /*boardData.ShouldNotBeNull();
      Tuple<ResourceTypes?, UInt32>[] hexes = boardData.GetHexInformation();
      hexes.Length.ShouldBe(GameBoard.StandardBoardHexCount);
      hexes[0].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 9));
      hexes[1].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 8));
      hexes[2].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 5));
      hexes[3].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 12));
      hexes[4].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 11));
      hexes[5].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 3));
      hexes[6].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 6));
      hexes[7].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 10));
      hexes[8].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 6));
      hexes[9].ShouldBe(new Tuple<ResourceTypes?, UInt32>(null, 0));
      hexes[10].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 4));
      hexes[11].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 11));
      hexes[12].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 2));
      hexes[13].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 4));
      hexes[14].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 3));
      hexes[15].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 5));
      hexes[16].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 9));
      hexes[17].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 10));
      hexes[18].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 8));*/
    }
  }
}
