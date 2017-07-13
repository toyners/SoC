
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  public class ComputerPlayer : IComputerPlayer
  {
    public Guid Id { get; private set; }

    public ComputerPlayer(Guid id)
    {
      this.Id = id;
      //this.Data = new PlayerData();
    }

    public Road ChooseRoad(GameBoardData gameBoardData)
    {
      var settlementsForPlayer = gameBoardData.GetSettlementsForPlayer(this.Id);
      if (settlementsForPlayer == null || settlementsForPlayer.Count == 0)
      {
        throw new Exception("No settlements found for player with id " + this.Id);
      }

      var locationIndex = settlementsForPlayer[0];
      var path = this.GetPathToLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, locationIndex);

      return new Road(locationIndex, path[path.Count - 1]);
    }

    public UInt32 ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      // Find location that has the highest chance of a return for any roll.
      var bestLocationIndex = 0u;
      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
      {
        throw new Exception("Should not get here"); //TODO: Clean up
      }

      return bestLocationIndex;
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

    private Boolean TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(GameBoardData gameBoardData, out UInt32 bestLocationIndex)
    {
      // Find location that has the highest chance of a return for any roll.
      var bestChanceOfReturnOnRoll = 0;
      var gotBestLocationIndex = false;
      bestLocationIndex = 0;

      // Iterate over every location and determine the chance of return for all resource providers
      for (UInt32 index = 0; index < gameBoardData.Locations.Length; index++)
      {
        //Guid playerId;
        var canPlaceResult = gameBoardData.CanPlaceSettlement(index);
        if (canPlaceResult.Status != GameBoardData.VerificationStatus.Valid)
        {
          continue;
        }

        var location = gameBoardData.Locations[index];
        var chanceOfReturnOnRoll = this.CalculateChangeOfReturnOnRoll(location.Providers);
        if (chanceOfReturnOnRoll > bestChanceOfReturnOnRoll)
        {
          bestChanceOfReturnOnRoll = chanceOfReturnOnRoll;
          bestLocationIndex = index;
          gotBestLocationIndex = true;
        }
      }

      return gotBestLocationIndex;
    }

    private List<UInt32> GetPathToLocationThatHasBestChanceOfReturnOnRoll(GameBoardData gameBoardData, UInt32 locationIndex)
    {
      var bestLocationIndex = 0u;

      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
      {
        throw new Exception("Should not get here"); // TODO: Clean up
      }

      return gameBoardData.GetPathBetweenLocations(locationIndex, bestLocationIndex);
    }

    public PlayerDataView GetDataView()
    {
      var dataView = new PlayerDataView();

      dataView.Id = this.Id;
      dataView.ResourceCards = 0u;
      dataView.HiddenDevelopmentCards = 0;
      dataView.DisplayedDevelopmentCards = null;

      return dataView;
    }
  }
}
