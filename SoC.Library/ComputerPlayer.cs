﻿
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using Interfaces;
  using GameActions;
  using System.Xml;

  public class ComputerPlayer : Player, IComputerPlayer
  {
    private Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
    private GameBoard gameBoard;

    #region Construction
    private ComputerPlayer() { } // For use when inflating from file. 

    public ComputerPlayer(String name, GameBoard gameBoard) : base(name)
    {
      this.gameBoard = gameBoard;
    }
    #endregion

    #region Properties
    public override Boolean IsComputer { get { return true; } }
    #endregion

    #region Methods
    public static ComputerPlayer CreateFromXML(XmlReader reader)
    {
      var computerPlayer = new ComputerPlayer();
      computerPlayer.Load(reader);
      return computerPlayer;
    }

    public virtual void AddDevelopmentCard(DevelopmentCard developmentCard)
    {
      throw new NotImplementedException();
    }

    public virtual void BuildInitialPlayerActions(GameBoard gameBoard, PlayerDataView[] playerData)
    {
      throw new NotImplementedException();
    }

    public virtual UInt32 ChooseCityLocation(GameBoard gameBoardData)
    {
      throw new NotImplementedException();
    }

    public virtual void ChooseInitialInfrastructure(out UInt32 settlementLocation, out UInt32 roadEndLocation)
    {
      throw new NotImplementedException();
    }

    public virtual KnightDevelopmentCard ChooseKnightCard()
    {
      throw new NotImplementedException();
    }

    public virtual IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers)
    {
      throw new NotImplementedException();
    }

    public virtual MonopolyDevelopmentCard ChooseMonopolyCard()
    {
      throw new NotImplementedException();
    }

    private void ChooseRoad(GameBoard gameBoardData, out UInt32 roadStartLocation, out UInt32 roadEndLocation)
    {
      var settlementsForPlayer = gameBoardData.GetSettlementsForPlayer(this.Id);
      if (settlementsForPlayer == null || settlementsForPlayer.Count == 0)
      {
        throw new Exception("No settlements found for player with id " + this.Id);
      }

      UInt32 bestLocationIndex = 0;
      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
      {
        throw new Exception("Should not get here"); // TODO: Clean up
      }

      Tuple<UInt32, List<UInt32>> shortestPathInformation = null;
      foreach (var locationIndex in settlementsForPlayer)
      {
        var path = gameBoardData.GetPathBetweenLocations(locationIndex, bestLocationIndex);

        if (shortestPathInformation == null || shortestPathInformation.Item2.Count > path.Count)
        {
          shortestPathInformation = new Tuple<UInt32, List<UInt32>>(locationIndex, path);
        }
      }

      roadStartLocation = shortestPathInformation.Item1;
      var shortestPath = shortestPathInformation.Item2;
      roadEndLocation = shortestPath[shortestPath.Count - 1];
    }

    public virtual UInt32 ChooseRobberLocation()
    {
      throw new NotImplementedException();
    }

    public virtual ResourceClutch ChooseResouresToCollectFromBank()
    {
      throw new NotImplementedException();
    }

    public virtual ResourceClutch ChooseResourcesToDrop()
    {
      throw new NotImplementedException();
    }

    public virtual ResourceTypes ChooseResourceTypeToRob()
    {
      throw new NotImplementedException();
    }

    public virtual UInt32 ChooseSettlementLocation(GameBoard gameBoardData)
    {
      // Find location that has the highest chance of a return for any roll.
      var bestLocationIndex = 0u;
      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
      {
        throw new Exception("Should not get here"); //TODO: Clean up
      }

      return bestLocationIndex;
    }

    public virtual YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard()
    {
      throw new NotImplementedException();
    }

    public void DropResources(Int32 resourceCount)
    {
      throw new NotImplementedException();
    }

    public virtual ComputerPlayerAction GetPlayerAction()
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
          case 12:
          totalChance += 1;
          break;
          case 3:
          case 11:
          totalChance += 2;
          break;
          case 4:
          case 10:
          totalChance += 3;
          break;
          case 5:
          case 9:
          totalChance += 4;
          break;
          case 6:
          case 8:
          totalChance += 5;
          break;
        }
      }

      return totalChance;
    }

    private Boolean TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(GameBoard gameBoardData, out UInt32 bestLocationIndex)
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
        if (canPlaceResult.Status == GameBoard.VerificationStatus.LocationForSettlementIsInvalid ||
            canPlaceResult.Status == GameBoard.VerificationStatus.LocationIsOccupied ||
            canPlaceResult.Status == GameBoard.VerificationStatus.TooCloseToSettlement)
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

    private List<UInt32> GetPathToLocationThatHasBestChanceOfReturnOnRoll(GameBoard gameBoardData, UInt32 locationIndex)
    {
      var bestLocationIndex = 0u;

      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
      {
        throw new Exception("Should not get here"); // TODO: Clean up
      }

      return gameBoardData.GetPathBetweenLocations(locationIndex, bestLocationIndex);
    }
    #endregion
  }
}
