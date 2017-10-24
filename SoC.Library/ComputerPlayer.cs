
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  public class ComputerPlayer : Player, IComputerPlayer
  {
    public ComputerPlayer() { }
    public ComputerPlayer(String name) : base(name) { }

    public override Boolean IsComputer { get { return true; } }

    public virtual void ChooseRoad(GameBoardData gameBoardData, out UInt32 roadStartLocation, out UInt32 roadEndLocation)
    {
      var settlementsForPlayer = gameBoardData.GetSettlementsForPlayer(this.Id);
      if (settlementsForPlayer == null || settlementsForPlayer.Count == 0)
      {
        throw new Exception("No settlements found for player with id " + this.Id);
      }

      var locationIndex = settlementsForPlayer[0];
      var path = this.GetPathToLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, locationIndex);

      roadStartLocation = locationIndex;
      roadEndLocation = path[path.Count - 1];
    }

    public virtual ResourceClutch ChooseResourcesToDrop()
    {
      throw new NotImplementedException();
    }

    public virtual UInt32 ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      // Find location that has the highest chance of a return for any roll.
      var bestLocationIndex = 0u;
      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
      {
        throw new Exception("Should not get here"); //TODO: Clean up
      }

      return bestLocationIndex;
    }

    public void DropResources(Int32 resourceCount)
    {
      throw new NotImplementedException();
    }

    private Int32 CalculateChanceOfReturnOnRoll(UInt32[] productionValues)
    {
      Int32 totalChance = 0;
      foreach (var productionValue in productionValues)
      {
        switch (productionValue)
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
      for (UInt32 index = 0; index < gameBoardData.Length; index++)
      {
        //Guid playerId;
        var canPlaceResult = gameBoardData.CanPlaceSettlement(this.Id, index);
        if (canPlaceResult.Status != GameBoardData.VerificationStatus.Valid)
        {
          continue;
        }

        var productionValues = gameBoardData.GetProductionValuesForLocation(index);
        var chanceOfReturnOnRoll = this.CalculateChanceOfReturnOnRoll(productionValues);
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
  }
}
