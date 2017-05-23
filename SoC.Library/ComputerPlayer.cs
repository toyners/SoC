
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  public class ComputerPlayer : IComputerPlayer
  {
    public Trail ChooseRoad(GameBoardData gameBoardData)
    {
      throw new NotImplementedException();
    }

    public Location ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      // Find location that has the highest chance of a return for any roll.
      var bestChanceOfReturnOnRoll = 0;
      var bestLocationSoFar = -1; 

      // Iterate over every location and determine the chance of return for all resource providers
      for (var index = 0; index < gameBoardData.Locations.Length; index++)
      {
        var location = gameBoardData.Locations[index];
        var chanceOfReturnOnRoll = this.CalculateChangeOfReturnOnRoll(location.Providers);
        System.Diagnostics.Debug.Write(String.Format("{0}: {1}", index, chanceOfReturnOnRoll));
        if (chanceOfReturnOnRoll > bestChanceOfReturnOnRoll)
        {
          bestChanceOfReturnOnRoll = chanceOfReturnOnRoll;
          bestLocationSoFar = index;
          System.Diagnostics.Debug.Write(" <= New High");
        }

        System.Diagnostics.Debug.WriteLine("");
      }

      return gameBoardData.Locations[bestLocationSoFar];
    }

    private Int32 CalculateChangeOfReturnOnRoll(HashSet<ResourceProvider> providers)
    {
      Int32 totalChance = 0;
      foreach (var provider in providers)
      {
        switch (provider.ProductionNumber)
        {
          case 2:
          case 12: totalChance += 1; break;
          case 3:
          case 11: totalChance += 2; break;
          case 4:
          case 10: totalChance += 3; break;
          case 5:
          case 9: totalChance += 4; break;
          case 6:
          case 8: totalChance += 5; break;
        }
      }

      return totalChance;
    }
  }
}
