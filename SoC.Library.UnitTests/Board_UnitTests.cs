
namespace Jabberwocky.SoC.Library.UnitTests
{
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Board_UnitTests
  {
    [Test]
    public void Create_StandardSize_LocationsCreated()
    {
      // Arrange and Act
      Board board = new Board(BoardSizes.Standard);

      // Assert
      board.Locations.Length.ShouldBe(Board.StandardBoardLocationCount);
    }

    [Test]
    public void Create_StandardSize_TrailsCreatedAndLinkedToLocations()
    {
      // Arrange and Act
      Board board = new Board(BoardSizes.Standard);

      // Assert
      // Columns of hexes 3-4-5-4-3
      // Column side 1 = 7 (hex count * 2 + 1)
      // Column side 2 = 9
      // Column side 3 = 11
      // Column side 4 = 11
      // Column side 5 = 9
      // Column side 6 = 7

      // Column side 1 = 6 (hex count * 2)
      // Column side 2 = 8
      // Column side 3 = 10
      // Column side 4 = 10
      // Column side 5 = 8
      // Column side 6 = 6

      // Column 1 horizontals = 4 (hex count + 1)
      // Column 2 horizontals = 5
      // Column 3 horizontals = 6
      // Column 4 horizontals = 5
      // Column 5 horizontals = 4
      board.Trails.Length.ShouldBe(Board.StandardBoardTrailCount);

      // Column side 1
      var locationIndex = 0;
      var trailIndex = 0;
      for (; trailIndex < 6; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 1]);
        locationIndex++;
      }

      // Column side 2
      locationIndex++;
      for (; trailIndex < 14; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 1]);
        locationIndex++;
      }

      // Column side 3
      locationIndex++;
      for (; trailIndex < 24; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 1]);
        locationIndex++;
      }

      // Column side 4
      locationIndex++;
      for (; trailIndex < 34; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 1]);
        locationIndex++;
      }

      // Column side 5
      locationIndex++;
      for (; trailIndex < 42; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 1]);
        locationIndex++;
      }

      // Column side 6
      locationIndex++;
      for (; trailIndex < 48; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 1]);
        locationIndex++;
      }

      // Column 1 horizontals (4)
      locationIndex = 0;
      for (; trailIndex < 52; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 8]);
        locationIndex += 2;
      }

      //Column 2 horizontals (5)
      locationIndex = 7;
      for (; trailIndex < 57; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 10]);
        locationIndex += 2;
      }

      //Column 3 horizontals (6)
      locationIndex = 16;
      for (; trailIndex < 63; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 11]);
        locationIndex += 2;
      }

      //Column 4 horizontals (5)
      locationIndex = 28;
      for (; trailIndex < 63; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 10]);
        locationIndex += 2;
      }

      //Column 5 horizontals (4)
      locationIndex = 39;
      for (; trailIndex < 63; trailIndex++)
      {
        board.Trails[trailIndex].Location1.ShouldBe(board.Locations[locationIndex]);
        board.Trails[trailIndex].Location2.ShouldBe(board.Locations[locationIndex + 8]);
        locationIndex += 2;
      }
    }

    [Test]
    public void Create_StandardSize_LocationsLinkedToTrails()
    {
      // Arrange and Act
      Board board = new Board(BoardSizes.Standard);

      // Assert
      // Column side 1
      board.Locations[0].Trails.Count.ShouldBe(2);
      board.Locations[0].Trails.ShouldContain(board.Trails[0]);

      board.Locations[1].Trails.Count.ShouldBe(2);
      board.Locations[1].Trails.ShouldContain(board.Trails[0]);
      board.Locations[1].Trails.ShouldContain(board.Trails[1]);

      board.Locations[2].Trails.Count.ShouldBe(3);
      board.Locations[2].Trails.ShouldContain(board.Trails[1]);
      board.Locations[2].Trails.ShouldContain(board.Trails[2]);

      board.Locations[3].Trails.Count.ShouldBe(2);
      board.Locations[3].Trails.ShouldContain(board.Trails[2]);
      board.Locations[3].Trails.ShouldContain(board.Trails[3]);

      board.Locations[4].Trails.Count.ShouldBe(3);
      board.Locations[4].Trails.ShouldContain(board.Trails[3]);
      board.Locations[4].Trails.ShouldContain(board.Trails[4]);

      board.Locations[5].Trails.Count.ShouldBe(2);
      board.Locations[5].Trails.ShouldContain(board.Trails[4]);
      board.Locations[5].Trails.ShouldContain(board.Trails[5]);

      board.Locations[6].Trails.Count.ShouldBe(2);
      board.Locations[6].Trails.ShouldContain(board.Trails[5]);

      // Column side 2
      board.Locations[7].Trails.Count.ShouldBe(2);
      board.Locations[7].Trails.ShouldContain(board.Trails[6]);

      board.Locations[8].Trails.Count.ShouldBe(3);
      board.Locations[8].Trails.ShouldContain(board.Trails[6]);
      board.Locations[8].Trails.ShouldContain(board.Trails[7]);

      board.Locations[9].Trails.Count.ShouldBe(3);
      board.Locations[9].Trails.ShouldContain(board.Trails[7]);
      board.Locations[9].Trails.ShouldContain(board.Trails[8]);

      board.Locations[10].Trails.Count.ShouldBe(3);
      board.Locations[10].Trails.ShouldContain(board.Trails[8]);
      board.Locations[10].Trails.ShouldContain(board.Trails[9]);

      board.Locations[11].Trails.Count.ShouldBe(3);
      board.Locations[11].Trails.ShouldContain(board.Trails[9]);
      board.Locations[11].Trails.ShouldContain(board.Trails[10]);

      board.Locations[12].Trails.Count.ShouldBe(3);
      board.Locations[12].Trails.ShouldContain(board.Trails[10]);
      board.Locations[12].Trails.ShouldContain(board.Trails[11]);

      board.Locations[13].Trails.Count.ShouldBe(3);
      board.Locations[13].Trails.ShouldContain(board.Trails[11]);
      board.Locations[13].Trails.ShouldContain(board.Trails[12]);

      board.Locations[14].Trails.Count.ShouldBe(3);
      board.Locations[14].Trails.ShouldContain(board.Trails[12]);
      board.Locations[14].Trails.ShouldContain(board.Trails[13]);

      board.Locations[15].Trails.Count.ShouldBe(2);
      board.Locations[15].Trails.ShouldContain(board.Trails[13]);

      // Column Side 3
      board.Locations[16].Trails.Count.ShouldBe(2);
      board.Locations[16].Trails.ShouldContain(board.Trails[14]);

      board.Locations[17].Trails.Count.ShouldBe(3);
      board.Locations[17].Trails.ShouldContain(board.Trails[14]);
      board.Locations[17].Trails.ShouldContain(board.Trails[15]);

      board.Locations[18].Trails.Count.ShouldBe(3);
      board.Locations[18].Trails.ShouldContain(board.Trails[15]);
      board.Locations[18].Trails.ShouldContain(board.Trails[16]);

      board.Locations[19].Trails.Count.ShouldBe(3);
      board.Locations[19].Trails.ShouldContain(board.Trails[16]);
      board.Locations[19].Trails.ShouldContain(board.Trails[17]);

      board.Locations[20].Trails.Count.ShouldBe(3);
      board.Locations[20].Trails.ShouldContain(board.Trails[17]);
      board.Locations[20].Trails.ShouldContain(board.Trails[18]);

      board.Locations[21].Trails.Count.ShouldBe(3);
      board.Locations[21].Trails.ShouldContain(board.Trails[18]);
      board.Locations[21].Trails.ShouldContain(board.Trails[19]);

      board.Locations[22].Trails.Count.ShouldBe(3);
      board.Locations[22].Trails.ShouldContain(board.Trails[19]);
      board.Locations[22].Trails.ShouldContain(board.Trails[20]);

      board.Locations[23].Trails.Count.ShouldBe(3);
      board.Locations[23].Trails.ShouldContain(board.Trails[20]);
      board.Locations[23].Trails.ShouldContain(board.Trails[21]);

      board.Locations[24].Trails.Count.ShouldBe(3);
      board.Locations[24].Trails.ShouldContain(board.Trails[21]);
      board.Locations[24].Trails.ShouldContain(board.Trails[22]);

      board.Locations[25].Trails.Count.ShouldBe(3);
      board.Locations[25].Trails.ShouldContain(board.Trails[22]);
      board.Locations[25].Trails.ShouldContain(board.Trails[23]);

      board.Locations[26].Trails.Count.ShouldBe(2);
      board.Locations[26].Trails.ShouldContain(board.Trails[23]);

      // Column Side 4
      board.Locations[27].Trails.Count.ShouldBe(2);
      board.Locations[27].Trails.ShouldContain(board.Trails[24]);

      board.Locations[28].Trails.Count.ShouldBe(3);
      board.Locations[28].Trails.ShouldContain(board.Trails[24]);
      board.Locations[28].Trails.ShouldContain(board.Trails[25]);

      board.Locations[29].Trails.Count.ShouldBe(3);
      board.Locations[29].Trails.ShouldContain(board.Trails[25]);
      board.Locations[29].Trails.ShouldContain(board.Trails[26]);

      board.Locations[30].Trails.Count.ShouldBe(3);
      board.Locations[30].Trails.ShouldContain(board.Trails[26]);
      board.Locations[30].Trails.ShouldContain(board.Trails[27]);

      board.Locations[31].Trails.Count.ShouldBe(3);
      board.Locations[31].Trails.ShouldContain(board.Trails[27]);
      board.Locations[31].Trails.ShouldContain(board.Trails[28]);

      board.Locations[32].Trails.Count.ShouldBe(3);
      board.Locations[32].Trails.ShouldContain(board.Trails[28]);
      board.Locations[32].Trails.ShouldContain(board.Trails[29]);

      board.Locations[33].Trails.Count.ShouldBe(3);
      board.Locations[33].Trails.ShouldContain(board.Trails[29]);
      board.Locations[33].Trails.ShouldContain(board.Trails[30]);

      board.Locations[34].Trails.Count.ShouldBe(3);
      board.Locations[34].Trails.ShouldContain(board.Trails[30]);
      board.Locations[34].Trails.ShouldContain(board.Trails[31]);

      board.Locations[35].Trails.Count.ShouldBe(3);
      board.Locations[35].Trails.ShouldContain(board.Trails[31]);
      board.Locations[35].Trails.ShouldContain(board.Trails[32]);

      board.Locations[36].Trails.Count.ShouldBe(3);
      board.Locations[36].Trails.ShouldContain(board.Trails[32]);
      board.Locations[36].Trails.ShouldContain(board.Trails[33]);

      board.Locations[37].Trails.Count.ShouldBe(2);
      board.Locations[37].Trails.ShouldContain(board.Trails[33]);

      // Column Side 5
      board.Locations[38].Trails.Count.ShouldBe(2);
      board.Locations[38].Trails.ShouldContain(board.Trails[34]);

      board.Locations[39].Trails.Count.ShouldBe(3);
      board.Locations[39].Trails.ShouldContain(board.Trails[34]);
      board.Locations[39].Trails.ShouldContain(board.Trails[35]);

      board.Locations[40].Trails.Count.ShouldBe(3);
      board.Locations[40].Trails.ShouldContain(board.Trails[35]);
      board.Locations[40].Trails.ShouldContain(board.Trails[36]);

      board.Locations[41].Trails.Count.ShouldBe(3);
      board.Locations[41].Trails.ShouldContain(board.Trails[36]);
      board.Locations[41].Trails.ShouldContain(board.Trails[37]);

      board.Locations[42].Trails.Count.ShouldBe(3);
      board.Locations[42].Trails.ShouldContain(board.Trails[37]);
      board.Locations[42].Trails.ShouldContain(board.Trails[38]);

      board.Locations[43].Trails.Count.ShouldBe(3);
      board.Locations[43].Trails.ShouldContain(board.Trails[38]);
      board.Locations[43].Trails.ShouldContain(board.Trails[39]);

      board.Locations[44].Trails.Count.ShouldBe(3);
      board.Locations[44].Trails.ShouldContain(board.Trails[39]);
      board.Locations[44].Trails.ShouldContain(board.Trails[40]);

      board.Locations[45].Trails.Count.ShouldBe(3);
      board.Locations[45].Trails.ShouldContain(board.Trails[40]);
      board.Locations[45].Trails.ShouldContain(board.Trails[41]);

      board.Locations[46].Trails.Count.ShouldBe(2);
      board.Locations[46].Trails.ShouldContain(board.Trails[41]);

      // Column Side 6
      board.Locations[47].Trails.Count.ShouldBe(2);
      board.Locations[47].Trails.ShouldContain(board.Trails[42]);

      board.Locations[48].Trails.Count.ShouldBe(2);
      board.Locations[48].Trails.ShouldContain(board.Trails[42]);
      board.Locations[48].Trails.ShouldContain(board.Trails[43]);

      board.Locations[49].Trails.Count.ShouldBe(3);
      board.Locations[49].Trails.ShouldContain(board.Trails[43]);
      board.Locations[49].Trails.ShouldContain(board.Trails[44]);

      board.Locations[50].Trails.Count.ShouldBe(2);
      board.Locations[50].Trails.ShouldContain(board.Trails[44]);
      board.Locations[50].Trails.ShouldContain(board.Trails[45]);

      board.Locations[51].Trails.Count.ShouldBe(3);
      board.Locations[51].Trails.ShouldContain(board.Trails[45]);
      board.Locations[51].Trails.ShouldContain(board.Trails[45]);

      board.Locations[52].Trails.Count.ShouldBe(2);
      board.Locations[52].Trails.ShouldContain(board.Trails[46]);
      board.Locations[52].Trails.ShouldContain(board.Trails[47]);

      board.Locations[53].Trails.Count.ShouldBe(2);
      board.Locations[53].Trails.ShouldContain(board.Trails[47]);

      // Column 1 Horizontals
      board.Locations[0].Trails.ShouldContain(board.Trails[48]);
      board.Locations[8].Trails.ShouldContain(board.Trails[48]);

      board.Locations[2].Trails.ShouldContain(board.Trails[49]);
      board.Locations[10].Trails.ShouldContain(board.Trails[49]);

      board.Locations[4].Trails.ShouldContain(board.Trails[50]);
      board.Locations[12].Trails.ShouldContain(board.Trails[50]);

      board.Locations[6].Trails.ShouldContain(board.Trails[51]);
      board.Locations[14].Trails.ShouldContain(board.Trails[51]);

      // Column 2 Horizontals
      board.Locations[7].Trails.ShouldContain(board.Trails[52]);
      board.Locations[17].Trails.ShouldContain(board.Trails[52]);

      board.Locations[9].Trails.ShouldContain(board.Trails[53]);
      board.Locations[19].Trails.ShouldContain(board.Trails[53]);

      board.Locations[11].Trails.ShouldContain(board.Trails[54]);
      board.Locations[21].Trails.ShouldContain(board.Trails[54]);

      board.Locations[13].Trails.ShouldContain(board.Trails[55]);
      board.Locations[23].Trails.ShouldContain(board.Trails[55]);

      board.Locations[15].Trails.ShouldContain(board.Trails[56]);
      board.Locations[25].Trails.ShouldContain(board.Trails[56]);

      // Column 3 Horizontals
      board.Locations[16].Trails.ShouldContain(board.Trails[57]);
      board.Locations[27].Trails.ShouldContain(board.Trails[57]);

      board.Locations[18].Trails.ShouldContain(board.Trails[58]);
      board.Locations[29].Trails.ShouldContain(board.Trails[58]);

      board.Locations[20].Trails.ShouldContain(board.Trails[59]);
      board.Locations[31].Trails.ShouldContain(board.Trails[59]);

      board.Locations[22].Trails.ShouldContain(board.Trails[60]);
      board.Locations[33].Trails.ShouldContain(board.Trails[60]);

      board.Locations[24].Trails.ShouldContain(board.Trails[61]);
      board.Locations[35].Trails.ShouldContain(board.Trails[61]);

      board.Locations[26].Trails.ShouldContain(board.Trails[62]);
      board.Locations[37].Trails.ShouldContain(board.Trails[62]);

      // Column 4 Horizontals
      board.Locations[28].Trails.ShouldContain(board.Trails[63]);
      board.Locations[38].Trails.ShouldContain(board.Trails[63]);

      board.Locations[30].Trails.ShouldContain(board.Trails[64]);
      board.Locations[40].Trails.ShouldContain(board.Trails[64]);

      board.Locations[32].Trails.ShouldContain(board.Trails[65]);
      board.Locations[42].Trails.ShouldContain(board.Trails[65]);

      board.Locations[34].Trails.ShouldContain(board.Trails[66]);
      board.Locations[44].Trails.ShouldContain(board.Trails[66]);

      board.Locations[36].Trails.ShouldContain(board.Trails[67]);
      board.Locations[46].Trails.ShouldContain(board.Trails[67]);

      // Column 5 Horizontals 
      board.Locations[39].Trails.ShouldContain(board.Trails[68]);
      board.Locations[47].Trails.ShouldContain(board.Trails[68]);

      board.Locations[41].Trails.ShouldContain(board.Trails[69]);
      board.Locations[49].Trails.ShouldContain(board.Trails[69]);

      board.Locations[43].Trails.ShouldContain(board.Trails[70]);
      board.Locations[51].Trails.ShouldContain(board.Trails[70]);

      board.Locations[45].Trails.ShouldContain(board.Trails[71]);
      board.Locations[53].Trails.ShouldContain(board.Trails[71]);
    }

    [Test]
    public void Create_StandardSize_ResourceProvidersLinkedToLocations()
    {
      // Arrange and Act
      Board board = new Board(BoardSizes.Standard);

      var desert = new ResourceProvider();
      var brick8 = new ResourceProvider(ResourceTypes.Brick, 8);
      var ore5 = new ResourceProvider(ResourceTypes.Ore, 5);
      var brick4 = new ResourceProvider(ResourceTypes.Brick, 4);
      var lumber3 = new ResourceProvider(ResourceTypes.Lumber, 3);
      var wool10 = new ResourceProvider(ResourceTypes.Wool, 10);
      var grain2 = new ResourceProvider(ResourceTypes.Grain, 2);
      var lumber11 = new ResourceProvider(ResourceTypes.Lumber, 11);
      var ore6 = new ResourceProvider(ResourceTypes.Ore, 6);
      var grain11 = new ResourceProvider(ResourceTypes.Grain, 11);
      var wool9 = new ResourceProvider(ResourceTypes.Wool, 9);
      var lumber6 = new ResourceProvider(ResourceTypes.Lumber, 6);
      var wool12 = new ResourceProvider(ResourceTypes.Wool, 12);
      var brick5 = new ResourceProvider(ResourceTypes.Brick, 5);
      var lumber4 = new ResourceProvider(ResourceTypes.Lumber, 4);
      var ore3 = new ResourceProvider(ResourceTypes.Ore, 3);
      var grain9 = new ResourceProvider(ResourceTypes.Grain, 9);
      var grain8 = new ResourceProvider(ResourceTypes.Grain, 8);

      // Assert
      board.Locations[0].Providers.Count.ShouldBe(1);
      board.Locations[0].Providers.Contains(desert);
      board.Locations[1].Providers.Count.ShouldBe(1);
      board.Locations[1].Providers.Contains(desert);

      board.Locations[2].Providers.Count.ShouldBe(2);
      board.Locations[2].Providers.Contains(desert);
      board.Locations[2].Providers.Contains(brick8);

      board.Locations[3].Providers.Count.ShouldBe(1);
      board.Locations[3].Providers.Contains(brick8);

      board.Locations[4].Providers.Count.ShouldBe(2);
      board.Locations[4].Providers.Contains(brick8);
      board.Locations[4].Providers.Contains(ore5);

      board.Locations[5].Providers.Count.ShouldBe(1);
      board.Locations[5].Providers.Contains(ore5);

      board.Locations[6].Providers.Count.ShouldBe(1);
      board.Locations[6].Providers.Contains(ore5);

      board.Locations[7].Providers.Count.ShouldBe(1);
      board.Locations[7].Providers.Contains(brick4);

      board.Locations[8].Providers.Count.ShouldBe(2);
      board.Locations[8].Providers.Contains(brick4);
      board.Locations[8].Providers.Contains(desert);

      board.Locations[9].Providers.Count.ShouldBe(3);
      board.Locations[9].Providers.Contains(desert);
      board.Locations[9].Providers.Contains(brick4);
      board.Locations[9].Providers.Contains(lumber3);

      board.Locations[10].Providers.Count.ShouldBe(3);
      board.Locations[10].Providers.Contains(desert);
      board.Locations[10].Providers.Contains(brick8);
      board.Locations[10].Providers.Contains(lumber3);

      board.Locations[11].Providers.Count.ShouldBe(3);
      board.Locations[11].Providers.Contains(wool10);
      board.Locations[11].Providers.Contains(brick8);
      board.Locations[11].Providers.Contains(lumber3);

      board.Locations[12].Providers.Count.ShouldBe(3);
      board.Locations[12].Providers.Contains(ore5);
      board.Locations[12].Providers.Contains(brick8);
      board.Locations[12].Providers.Contains(wool10);

      board.Locations[13].Providers.Count.ShouldBe(3);
      board.Locations[13].Providers.Contains(wool10);
      board.Locations[13].Providers.Contains(ore5);
      board.Locations[13].Providers.Contains(grain2);

      board.Locations[14].Providers.Count.ShouldBe(2);
      board.Locations[14].Providers.Contains(ore5);
      board.Locations[14].Providers.Contains(grain2);

      board.Locations[15].Providers.Count.ShouldBe(1);
      board.Locations[15].Providers.Contains(grain2);

      board.Locations[16].Providers.Count.ShouldBe(1);
      board.Locations[16].Providers.Contains(lumber11);

      board.Locations[17].Providers.Count.ShouldBe(2);
      board.Locations[17].Providers.Contains(lumber11);
      board.Locations[17].Providers.Contains(brick4);

      board.Locations[18].Providers.Count.ShouldBe(3);
      board.Locations[18].Providers.Contains(lumber11);
      board.Locations[18].Providers.Contains(brick4);
      board.Locations[18].Providers.Contains(ore6);

      board.Locations[19].Providers.Count.ShouldBe(3);
      board.Locations[19].Providers.Contains(brick4);
      board.Locations[19].Providers.Contains(ore6);
      board.Locations[19].Providers.Contains(lumber3);

      board.Locations[20].Providers.Count.ShouldBe(3);
      board.Locations[20].Providers.Contains(grain11);
      board.Locations[20].Providers.Contains(ore6);
      board.Locations[20].Providers.Contains(lumber3);

      board.Locations[21].Providers.Count.ShouldBe(3);
      board.Locations[21].Providers.Contains(grain11);
      board.Locations[21].Providers.Contains(wool10);
      board.Locations[21].Providers.Contains(lumber3);

      board.Locations[22].Providers.Count.ShouldBe(3);
      board.Locations[22].Providers.Contains(grain11);
      board.Locations[22].Providers.Contains(wool9);
      board.Locations[22].Providers.Contains(wool10);

      board.Locations[23].Providers.Count.ShouldBe(3);
      board.Locations[23].Providers.Contains(wool10);
      board.Locations[23].Providers.Contains(grain2);
      board.Locations[23].Providers.Contains(wool9);

      board.Locations[24].Providers.Count.ShouldBe(3);
      board.Locations[24].Providers.Contains(wool9);
      board.Locations[24].Providers.Contains(lumber6);
      board.Locations[24].Providers.Contains(grain2);

      board.Locations[25].Providers.Count.ShouldBe(2);
      board.Locations[25].Providers.Contains(grain2);
      board.Locations[25].Providers.Contains(lumber6);

      board.Locations[26].Providers.Count.ShouldBe(1);
      board.Locations[26].Providers.Contains(lumber6);

      board.Locations[27].Providers.Count.ShouldBe(1);
      board.Locations[27].Providers.Contains(lumber11);

      board.Locations[28].Providers.Count.ShouldBe(2);
      board.Locations[28].Providers.Contains(wool12);
      board.Locations[28].Providers.Contains(lumber11);

      board.Locations[29].Providers.Count.ShouldBe(3);
      board.Locations[29].Providers.Contains(wool12);
      board.Locations[29].Providers.Contains(lumber11);
      board.Locations[29].Providers.Contains(ore6);

      board.Locations[30].Providers.Count.ShouldBe(3);
      board.Locations[30].Providers.Contains(wool12);
      board.Locations[30].Providers.Contains(brick5);
      board.Locations[30].Providers.Contains(ore6);

      board.Locations[31].Providers.Count.ShouldBe(3);
      board.Locations[31].Providers.Contains(grain11);
      board.Locations[31].Providers.Contains(brick5);
      board.Locations[31].Providers.Contains(ore6);

      board.Locations[32].Providers.Count.ShouldBe(3);
      board.Locations[32].Providers.Contains(grain11);
      board.Locations[32].Providers.Contains(brick5);
      board.Locations[32].Providers.Contains(lumber4);

      board.Locations[33].Providers.Count.ShouldBe(3);
      board.Locations[33].Providers.Contains(grain11);
      board.Locations[33].Providers.Contains(wool9);
      board.Locations[33].Providers.Contains(lumber4);

      board.Locations[34].Providers.Count.ShouldBe(3);
      board.Locations[34].Providers.Contains(ore3);
      board.Locations[34].Providers.Contains(wool9);
      board.Locations[34].Providers.Contains(lumber4);

      board.Locations[35].Providers.Count.ShouldBe(3);
      board.Locations[35].Providers.Contains(ore3);
      board.Locations[35].Providers.Contains(wool9);
      board.Locations[35].Providers.Contains(lumber6);

      board.Locations[36].Providers.Count.ShouldBe(2);
      board.Locations[36].Providers.Contains(ore3);
      board.Locations[36].Providers.Contains(lumber6);

      board.Locations[37].Providers.Count.ShouldBe(1);
      board.Locations[37].Providers.Contains(lumber6);

      board.Locations[38].Providers.Count.ShouldBe(1);
      board.Locations[38].Providers.Contains(wool12);

      board.Locations[39].Providers.Count.ShouldBe(2);
      board.Locations[39].Providers.Contains(wool12);
      board.Locations[39].Providers.Contains(grain9);

      board.Locations[40].Providers.Count.ShouldBe(3);
      board.Locations[40].Providers.Contains(wool12);
      board.Locations[40].Providers.Contains(grain9);
      board.Locations[40].Providers.Contains(brick5);

      board.Locations[41].Providers.Count.ShouldBe(3);
      board.Locations[41].Providers.Contains(wool10);
      board.Locations[41].Providers.Contains(grain9);
      board.Locations[41].Providers.Contains(brick5);

      board.Locations[42].Providers.Count.ShouldBe(3);
      board.Locations[42].Providers.Contains(wool10);
      board.Locations[42].Providers.Contains(lumber4);
      board.Locations[42].Providers.Contains(brick5);

      board.Locations[43].Providers.Count.ShouldBe(3);
      board.Locations[43].Providers.Contains(wool10);
      board.Locations[43].Providers.Contains(lumber4);
      board.Locations[43].Providers.Contains(grain8);

      board.Locations[44].Providers.Count.ShouldBe(3);
      board.Locations[44].Providers.Contains(ore3);
      board.Locations[44].Providers.Contains(lumber4);
      board.Locations[44].Providers.Contains(grain8);

      board.Locations[45].Providers.Count.ShouldBe(2);
      board.Locations[45].Providers.Contains(ore3);
      board.Locations[45].Providers.Contains(grain8);

      board.Locations[46].Providers.Count.ShouldBe(1);
      board.Locations[46].Providers.Contains(ore3);

      board.Locations[47].Providers.Count.ShouldBe(1);
      board.Locations[47].Providers.Contains(grain9);

      board.Locations[48].Providers.Count.ShouldBe(1);
      board.Locations[48].Providers.Contains(grain9);

      board.Locations[49].Providers.Count.ShouldBe(2);
      board.Locations[49].Providers.Contains(grain9);
      board.Locations[49].Providers.Contains(wool10);

      board.Locations[50].Providers.Count.ShouldBe(1);
      board.Locations[50].Providers.Contains(wool10);

      board.Locations[51].Providers.Count.ShouldBe(2);
      board.Locations[51].Providers.Contains(grain8);
      board.Locations[51].Providers.Contains(wool10);

      board.Locations[52].Providers.Count.ShouldBe(1);
      board.Locations[52].Providers.Contains(grain8);

      board.Locations[53].Providers.Count.ShouldBe(1);
      board.Locations[53].Providers.Contains(grain8);
    }
  }
}
