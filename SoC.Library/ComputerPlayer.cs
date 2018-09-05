
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using Interfaces;
  using GameActions;
  using System.Xml;
  using Jabberwocky.SoC.Library.Storage;
  using Jabberwocky.SoC.Library.Enums;

  public class ComputerPlayer : Player,  IComputerPlayer
  {
    private Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
    private readonly GameBoard gameBoard;
    private readonly INumberGenerator numberGenerator;
    private readonly List<uint> settlementCandidates = new List<uint>();
    private readonly DecisionMaker decisionMaker;

    #region Construction
    private ComputerPlayer() { } // For use when inflating from file. 
    
    public ComputerPlayer(String name, GameBoard gameBoard, INumberGenerator numberGenerator, IInfrastructureAI infrastructureAI) : base(name)
    {
      this.gameBoard = gameBoard;
      this.numberGenerator = numberGenerator;
      this.decisionMaker = new DecisionMaker(this.numberGenerator);
    }

    public ComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator) : base(data)
    {
      this.gameBoard = board;
      this.numberGenerator = numberGenerator;
      this.decisionMaker = new DecisionMaker(this.numberGenerator);
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

    public virtual void BuildInitialPlayerActions(PlayerDataView[] playerData)
    {
      this.decisionMaker.Reset();

      var resourceClutch = new ResourceClutch(this.BrickCount, this.GrainCount, this.LumberCount, this.OreCount, this.WoolCount);

      if (resourceClutch >= ResourceClutch.RoadSegment && this.RemainingRoadSegments > 0)
      {
        // Get the current road segment build candidates 
        var roadSegmentCandidates = this.gameBoard.BoardQuery.GetRoadSegmentCandidates(this.settlementCandidates);

        if (roadSegmentCandidates.Count > 1)
        {
          // Can build road - boost it if road builder strategy or if building the next road segment
          // will capture the 2VP
          uint multiplier = 1;

          this.decisionMaker.AddDecision(1, multiplier);
        }

        this.decisionMaker.AddDecision(1);
        var roadBuildSegmentAction = new BuildRoadSegmentAction(ComputerPlayerActionTypes.BuildRoadSegment, 0, 0);
        this.actions.Enqueue(roadBuildSegmentAction);
      }

      if (resourceClutch >= ResourceClutch.Settlement && this.RemainingSettlements > 0)
      {
        // Got the resources to build settlement. Find locations that can be build on 
        // Boost it if can build on target settlement - increase boost multiplier based on desirablity of target settlement
        // If building the settlement on any location will win the game then do it
      }

      if (resourceClutch >= ResourceClutch.City && this.RemainingCities > 0)
      {
        // Got resources to build city. Find settlements to promote
        // If building the city will win the game then do it
      }

      if (resourceClutch >= ResourceClutch.DevelopmentCard)
      {
        // Can buy development card
        // Boost it if playing development card buyer strategy
      }


    }

    public virtual UInt32 ChooseCityLocation()
    {
      throw new NotImplementedException();
    }

    public virtual void ChooseInitialInfrastructure(out UInt32 settlementLocation, out UInt32 roadEndLocation)
    {
      if (this.SettlementsBuilt >= 2)
      {
        throw new Exception("Should not get here");
      }

      var settlementIndex = -1;
      var bestLocations = this.gameBoard.BoardQuery.GetLocationsWithBestYield(5);
      var n = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(100);
        
      if (n < 55)
      {
        settlementIndex = 0;
      }
      else if (n < 75)
      {
        settlementIndex = 1;
      }
      else if (n < 85)
      {
        settlementIndex = 2;
      }
      else if (n < 95)
      {
        settlementIndex = 3;
      }
      else
      {
        settlementIndex = 4;
      }

      settlementLocation = bestLocations[settlementIndex];

      // Build road towards another random location 
      var roadDestinationIndex = -1;
      do
      {
        n = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(100);

        if (n < 55)
        {
          roadDestinationIndex = 0;
        }
        else if (n < 75)
        {
          roadDestinationIndex = 1;
        }
        else if (n < 85)
        {
          roadDestinationIndex = 2;
        }
        else if (n < 95)
        {
          roadDestinationIndex = 3;
        }
        else
        {
          roadDestinationIndex = 4;
        }

      } while (roadDestinationIndex == settlementIndex);

      var trek = this.gameBoard.GetPathBetweenLocations(settlementLocation, bestLocations[roadDestinationIndex]);

      roadEndLocation = trek[trek.Count - 1];
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

    public virtual UInt32 ChooseRobberLocation()
    {
      throw new NotImplementedException();
    }

    public virtual ResourceClutch ChooseResourcesToCollectFromBank()
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

    public virtual UInt32 ChooseSettlementLocation()
    {
      // Find location that has the highest chance of a return for any roll.
      var bestLocationIndex = 0u;
      if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(this.gameBoard, out bestLocationIndex))
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
