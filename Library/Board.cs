﻿
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class Board
  {
    #region Fields
    public Location[] Locations;

    public Trail[] Trails;

    public const Int32 StandardBoardLocationCount = 54;

    public const Int32 StandardBoardTrailCount = 72;
    #endregion

    #region Methods
    public void Create(BoardSizes size)
    {
      if (size == BoardSizes.Extended)
      {
        throw new Exception("Extended boards not implemented.");
      }

      this.CreateLocations();

      this.Trails = new Trail[StandardBoardTrailCount];

      var index = this.StitchLocationsTogetherUsingVerticalTrails();

      this.StitchLocationsTogetherUsingHorizontalTrails(index);

      this.CreateResourcesProvider();
    }

    public void PlaceStartingRoad(Location location1, Location location2) { }

    public void PlaceStartingSettlement(Location location) { }

    public void ProduceResources(UInt32 productionNumber)
    {

    }

    private void CreateLocations()
    {
      this.Locations = new Location[StandardBoardLocationCount];
      for (var index = 0; index < StandardBoardLocationCount; index++)
      {
        this.Locations[index] = new Location();
      }
    }

    private void CreateResourcesProvider()
    {
      var desert = new ResourceProvider();
      var brick8 = new ResourceProvider(ResourceTypes.Brick, 8);
      var ore5 = new ResourceProvider(ResourceTypes.Ore, 5);

      //b4,l3,w10,g2,
      var brick4 = new ResourceProvider(ResourceTypes.Brick, 4);
      var lumber3 = new ResourceProvider(ResourceTypes.Lumber, 3);
      var wool10 = new ResourceProvider(ResourceTypes.Wool, 10);
      var grain2 = new ResourceProvider(ResourceTypes.Grain, 2);

      //l11,o6,g11,w9,l6,
      var lumber11 = new ResourceProvider(ResourceTypes.Lumber, 11);
      var ore6 = new ResourceProvider(ResourceTypes.Ore, 6);
      var grain11 = new ResourceProvider(ResourceTypes.Grain, 11);
      var wool9 = new ResourceProvider(ResourceTypes.Wool, 9);
      var lumber6 = new ResourceProvider(ResourceTypes.Lumber, 6);

      //w12,b5,l4,o3
      var wool12 = new ResourceProvider(ResourceTypes.Wool, 12);
      var brick5 = new ResourceProvider(ResourceTypes.Brick, 5);
      var lumber4 = new ResourceProvider(ResourceTypes.Lumber, 4);
      var ore3 = new ResourceProvider(ResourceTypes.Ore, 3);

      //g9,w10,g8
      var grain9 = new ResourceProvider(ResourceTypes.Grain, 9);
      var grain8 = new ResourceProvider(ResourceTypes.Grain, 8);

      // Side 1
      this.Locations[0].Providers.Add(desert);
      this.Locations[1].Providers.Add(desert);
      this.Locations[2].Providers.Add(desert);
      this.Locations[2].Providers.Add(brick8);
      this.Locations[3].Providers.Add(brick8);
      this.Locations[4].Providers.Add(brick8);
      this.Locations[4].Providers.Add(ore5);
      this.Locations[5].Providers.Add(ore5);
      this.Locations[6].Providers.Add(ore5);

      // Side 2
      this.Locations[7].Providers.Add(brick4);
      this.Locations[8].Providers.Add(desert);
      this.Locations[8].Providers.Add(brick4);
      this.Locations[9].Providers.Add(brick4);
      this.Locations[9].Providers.Add(desert);
      this.Locations[9].Providers.Add(lumber3);
      this.Locations[10].Providers.Add(brick8);
      this.Locations[10].Providers.Add(desert);
      this.Locations[10].Providers.Add(lumber3);
      this.Locations[11].Providers.Add(brick8);
      this.Locations[11].Providers.Add(wool10);
      this.Locations[11].Providers.Add(lumber3);
      this.Locations[12].Providers.Add(brick8);
      this.Locations[12].Providers.Add(wool10);
      this.Locations[12].Providers.Add(ore5);
      this.Locations[13].Providers.Add(wool10);
      this.Locations[13].Providers.Add(ore5);
      this.Locations[13].Providers.Add(grain2);
      this.Locations[14].Providers.Add(ore5);
      this.Locations[14].Providers.Add(grain2);
      this.Locations[15].Providers.Add(grain2);

      // Side 3
      this.Locations[16].Providers.Add(lumber11);
      this.Locations[17].Providers.Add(lumber11);
      this.Locations[17].Providers.Add(brick4);
      this.Locations[18].Providers.Add(lumber11);
      this.Locations[18].Providers.Add(brick4);
      this.Locations[18].Providers.Add(ore6);
      this.Locations[19].Providers.Add(lumber3);
      this.Locations[19].Providers.Add(brick4);
      this.Locations[19].Providers.Add(ore6);
      this.Locations[20].Providers.Add(lumber3);
      this.Locations[20].Providers.Add(grain11);
      this.Locations[20].Providers.Add(ore6);
      this.Locations[21].Providers.Add(lumber3);
      this.Locations[21].Providers.Add(wool10);
      this.Locations[21].Providers.Add(grain11);
      this.Locations[22].Providers.Add(grain11);
      this.Locations[22].Providers.Add(wool10);
      this.Locations[22].Providers.Add(wool9);
      this.Locations[23].Providers.Add(grain2);
      this.Locations[23].Providers.Add(wool10);
      this.Locations[23].Providers.Add(wool9);
      this.Locations[24].Providers.Add(grain2);
      this.Locations[24].Providers.Add(lumber6);
      this.Locations[24].Providers.Add(wool9);
      this.Locations[25].Providers.Add(lumber6);
      this.Locations[25].Providers.Add(grain2);
      this.Locations[26].Providers.Add(lumber6);

      // Side 4
      this.Locations[27].Providers.Add(lumber11);
      this.Locations[28].Providers.Add(lumber11);
      this.Locations[28].Providers.Add(wool12);
      this.Locations[29].Providers.Add(lumber11);
      this.Locations[29].Providers.Add(wool12);
      this.Locations[29].Providers.Add(ore6);
      this.Locations[30].Providers.Add(wool12);
      this.Locations[30].Providers.Add(ore6);
      this.Locations[30].Providers.Add(brick5);
      this.Locations[31].Providers.Add(ore6);
      this.Locations[31].Providers.Add(brick5);
      this.Locations[31].Providers.Add(grain11);
      this.Locations[32].Providers.Add(brick5);
      this.Locations[32].Providers.Add(grain11);
      this.Locations[32].Providers.Add(lumber4);
      this.Locations[33].Providers.Add(grain11);
      this.Locations[33].Providers.Add(lumber4);
      this.Locations[33].Providers.Add(wool9);
      this.Locations[34].Providers.Add(lumber4);
      this.Locations[34].Providers.Add(wool9);
      this.Locations[34].Providers.Add(ore3);
      this.Locations[35].Providers.Add(wool9);
      this.Locations[35].Providers.Add(ore3);
      this.Locations[35].Providers.Add(lumber6);
      this.Locations[36].Providers.Add(lumber6);
      this.Locations[36].Providers.Add(ore3);
      this.Locations[37].Providers.Add(lumber6);

      // Side 5
      this.Locations[38].Providers.Add(wool12);
      this.Locations[39].Providers.Add(wool12);
      this.Locations[39].Providers.Add(grain9);
      this.Locations[40].Providers.Add(wool12);
      this.Locations[40].Providers.Add(grain9);
      this.Locations[40].Providers.Add(brick5);
      this.Locations[41].Providers.Add(grain9);
      this.Locations[41].Providers.Add(brick5);
      this.Locations[41].Providers.Add(wool10);
      this.Locations[42].Providers.Add(brick5);
      this.Locations[42].Providers.Add(wool10);
      this.Locations[42].Providers.Add(lumber4);
      this.Locations[43].Providers.Add(wool10);
      this.Locations[43].Providers.Add(lumber4);
      this.Locations[43].Providers.Add(grain8);
      this.Locations[44].Providers.Add(lumber4);
      this.Locations[44].Providers.Add(grain8);
      this.Locations[44].Providers.Add(ore3);
      this.Locations[45].Providers.Add(grain8);
      this.Locations[45].Providers.Add(ore3);
      this.Locations[46].Providers.Add(ore3);

      // Side 6
      this.Locations[47].Providers.Add(grain9);
      this.Locations[48].Providers.Add(grain9);
      this.Locations[49].Providers.Add(grain9);
      this.Locations[49].Providers.Add(wool10);
      this.Locations[50].Providers.Add(wool10);
      this.Locations[51].Providers.Add(wool10);
      this.Locations[51].Providers.Add(grain8);
      this.Locations[52].Providers.Add(grain8);
      this.Locations[53].Providers.Add(grain8);

    }

    private void StitchLocationsTogetherUsingHorizontalTrails(Int32 index)
    {
      // Add horizontal trails for columns
      foreach (var setup in new[] {
            new HorizontalTrailSetup(0, 4, 8),
            new HorizontalTrailSetup(7, 5, 10),
            new HorizontalTrailSetup(16, 6, 11),
            new HorizontalTrailSetup(28, 5, 10),
            new HorizontalTrailSetup(39, 4, 8) })
      {
        var count = setup.TrailCount;
        var locationIndex = setup.LocationIndexStart;
        while (count-- > 0)
        {
          var location1 = this.Locations[locationIndex];
          var location2 = this.Locations[locationIndex + setup.LocationIndexDiff];
          var trail = new Trail(location1, location2);
          this.Trails[index++] = trail;
          location1.AddTrail(trail);
          location2.AddTrail(trail);

          locationIndex += 2;
        }
      }
    }

    private Int32 StitchLocationsTogetherUsingVerticalTrails()
    {
      var index = 0;
      var locationIndex = 0;
      foreach (var trailCount in new[] { 6, 8, 10, 10, 8, 6 })
      {
        var count = trailCount;
        while (count-- > 0)
        {
          var location1 = this.Locations[locationIndex];
          var location2 = this.Locations[locationIndex + 1];
          var trail = new Trail(location1, location2);
          this.Trails[index++] = trail;
          location1.AddTrail(trail);
          location2.AddTrail(trail);

          locationIndex++;
        }

        locationIndex++;
      }

      return index;
    }
    #endregion

    #region Structs
    private struct HorizontalTrailSetup
    {
      public Int32 LocationIndexStart;
      public Int32 TrailCount;
      public Int32 LocationIndexDiff;

      public HorizontalTrailSetup(Int32 locationIndexStart, Int32 trailCount, Int32 locationIndexDiff)
      {
        this.LocationIndexStart = locationIndexStart;
        this.TrailCount = trailCount;
        this.LocationIndexDiff = locationIndexDiff;
      }
    }
    #endregion
  }
}
