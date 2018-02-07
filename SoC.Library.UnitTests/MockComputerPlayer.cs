
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  /// <summary>
  /// Used to set opponent player behaviour for testing purposes
  /// </summary>
  public class MockComputerPlayer : ComputerPlayer
  {
    #region Fields
    public UInt32 HiddenDevelopmentCards;
    public UInt32 ResourceCards;
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards;

    public Queue<UInt32> CityLocations = new Queue<UInt32>();
    private Dictionary<DevelopmentCardTypes, Queue<DevelopmentCard>> developmentCards = new Dictionary<DevelopmentCardTypes, Queue<DevelopmentCard>>();
    private Queue<PlayKnightInstruction> playKnightCardActions = new Queue<PlayKnightInstruction>();
    private Queue<PlayMonopolyCardInstruction> playMonopolyCardActions = new Queue<PlayMonopolyCardInstruction>();
    private Queue<PlayYearOfPlentyCardInstruction> playYearOfPlentyCardActions = new Queue<PlayYearOfPlentyCardInstruction>();
    private Queue<TradeWithBankInstruction> tradeWithBankInstructions = new Queue<TradeWithBankInstruction>();
    public Queue<UInt32> SettlementLocations = new Queue<UInt32>();
    public Queue<Tuple<UInt32, UInt32>> Roads = new Queue<Tuple<UInt32, UInt32>>();
    public Queue<Tuple<UInt32, UInt32>> InitialInfrastructure = new Queue<Tuple<UInt32, UInt32>>();
    public Queue<PlayerActionTypes> Actions = new Queue<PlayerActionTypes>();
    public ResourceClutch DroppedResources;
    #endregion

    #region Construction
    public MockComputerPlayer(String name) : base(name) { }
    #endregion

    #region Methods
    public MockComputerPlayer AddBuildCityInstruction(BuildCityInstruction instruction)
    {
      this.CityLocations.Enqueue(instruction.Location);
      this.Actions.Enqueue(PlayerActionTypes.BuildCity);
      return this;
    }

    public MockComputerPlayer AddBuildSettlementInstruction(BuildSettlementInstruction instruction)
    {
      this.SettlementLocations.Enqueue(instruction.Location);
      this.Actions.Enqueue(PlayerActionTypes.BuildSettlement);
      return this;
    }

    public void AddInitialInfrastructureChoices(UInt32 firstSettlmentLocation, UInt32 firstRoadEndLocation, UInt32 secondSettlementLocation, UInt32 secondRoadEndLocation)
    {
      this.InitialInfrastructure.Enqueue(new Tuple<UInt32, UInt32>(firstSettlmentLocation, firstRoadEndLocation));
      this.InitialInfrastructure.Enqueue(new Tuple<UInt32, UInt32>(secondSettlementLocation, secondRoadEndLocation));
    }
    
    public MockComputerPlayer AddBuyDevelopmentCardChoice(UInt32 count)
    {
      for (; count > 0; count--)
      {
        this.Actions.Enqueue(PlayerActionTypes.BuyDevelopmentCard);
      }

      return this;
    }

    public override void AddDevelopmentCard(DevelopmentCard developmentCard)
    {
      DevelopmentCardTypes? developmentCardType = null;

      if (developmentCard is KnightDevelopmentCard)
      {
        developmentCardType = DevelopmentCardTypes.Knight;
      }

      if (developmentCard is MonopolyDevelopmentCard)
      {
        developmentCardType = DevelopmentCardTypes.Monopoly;
      }

      if (developmentCard is YearOfPlentyDevelopmentCard)
      {
        developmentCardType = DevelopmentCardTypes.YearOfPlenty;
      }

      if (developmentCardType == null)
      {
        throw new Exception("Development card is not recognised.");
      }

      var key = developmentCardType.Value;
      if (!this.developmentCards.ContainsKey(key))
      {
        var queue = new Queue<DevelopmentCard>();
        queue.Enqueue(developmentCard);
        this.developmentCards.Add(key, queue);
      }
      else
      {
        this.developmentCards[key].Enqueue(developmentCard);
      }
    }

    public MockComputerPlayer AddBuildRoadSegmentInstruction(BuildRoadSegmentInstruction instruction)
    {
      for (var index = 0; index < instruction.Locations.Length; index += 2)
      {
        this.Roads.Enqueue(new Tuple<UInt32, UInt32>(instruction.Locations[index], instruction.Locations[index + 1]));
        this.Actions.Enqueue(PlayerActionTypes.BuildRoadSegment);
      }

      return this;
    }

    public MockComputerPlayer AddPlaceKnightCardInstruction(PlayKnightInstruction playKnightCardInstruction)
    {
      this.playKnightCardActions.Enqueue(playKnightCardInstruction);
      this.Actions.Enqueue(PlayerActionTypes.PlayKnightCard);
      return this;
    }

    public MockComputerPlayer AddPlaceMonopolyCardInstruction(PlayMonopolyCardInstruction playMonopolyCardInstruction)
    {
      this.playMonopolyCardActions.Enqueue(playMonopolyCardInstruction);
      this.Actions.Enqueue(PlayerActionTypes.PlayMonopolyCard);
      return this;
    }

    public MockComputerPlayer AddPlayYearOfPlentyCardInstruction(PlayYearOfPlentyCardInstruction playYearOfPlentyCardInstruction)
    {
      this.playYearOfPlentyCardActions.Enqueue(playYearOfPlentyCardInstruction);
      this.Actions.Enqueue(PlayerActionTypes.PlayYearOfPlentyCard);
      return this;
    }

    public MockComputerPlayer AddTradeWithBankInstruction(TradeWithBankInstruction tradeWithBankInstruction)
    {
      this.tradeWithBankInstructions.Enqueue(tradeWithBankInstruction);
      this.Actions.Enqueue(PlayerActionTypes.TradeWithBank);
      return this;
    }

    public override UInt32 ChooseCityLocation(GameBoardData gameBoardData)
    {
      return this.CityLocations.Dequeue();
    }

    public override void ChooseInitialInfrastructure(GameBoardData gameBoardData, out UInt32 settlementLocation, out UInt32 roadEndLocation)
    {
      var infrastructure = this.InitialInfrastructure.Dequeue();
      settlementLocation = infrastructure.Item1;
      roadEndLocation = infrastructure.Item2;
    }

    public override KnightDevelopmentCard ChooseKnightCard()
    {
      return this.developmentCards[DevelopmentCardTypes.Knight].Dequeue() as KnightDevelopmentCard;
    }

    public override IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers)
    {
      var action = this.playKnightCardActions.Dequeue();
      foreach (var otherPlayer in otherPlayers)
      {
        if (otherPlayer.Id == action.RobbedPlayerId)
        {
          return otherPlayer;
        }
      }

      throw new Exception("Cannot find player with Id '" + action.RobbedPlayerId + "' when choosing player to rob.");
    }

    public override MonopolyDevelopmentCard ChooseMonopolyCard()
    {
      return this.developmentCards[DevelopmentCardTypes.Monopoly].Dequeue() as MonopolyDevelopmentCard;
    }

    public override ResourceClutch ChooseResouresToCollectFromBank()
    {
      var action = this.playYearOfPlentyCardActions.Dequeue();
      
      var resources = ResourceClutch.Zero;
      foreach (var resourceChoice in new[] { action.FirstResourceChoice, action.SecondResourceChoice })
      { 
        switch (resourceChoice)
        {
          case ResourceTypes.Brick: resources.BrickCount++; break;
          case ResourceTypes.Lumber: resources.LumberCount++; break;
          case ResourceTypes.Grain: resources.GrainCount++; break;
          case ResourceTypes.Ore: resources.OreCount++; break;
          case ResourceTypes.Wool: resources.WoolCount++; break;
        }
      }

      return resources;
    }

    public override ResourceClutch ChooseResourcesToDrop()
    {
      return DroppedResources;
    }

    public override ResourceTypes ChooseResourceTypeToRob()
    {
      var action = this.playMonopolyCardActions.Dequeue();
      return action.ResourceType;
    }

    public override UInt32 ChooseRobberLocation()
    {
      return this.playKnightCardActions.Peek().RobberHex;
    }

    public override void ChooseRoad(GameBoardData gameBoardData, out UInt32 roadStartLocation, out UInt32 roadEndLocation)
    {
      var roadLocations = this.Roads.Dequeue();
      roadStartLocation = roadLocations.Item1;
      roadEndLocation = roadLocations.Item2;
    }

    public override UInt32 ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      return this.SettlementLocations.Dequeue();
    }

    public override YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard()
    {
      return this.developmentCards[DevelopmentCardTypes.YearOfPlenty].Dequeue() as YearOfPlentyDevelopmentCard;
    }

    public MockComputerPlayer EndTurn()
    {
      this.Actions.Enqueue(PlayerActionTypes.EndTurn);
      return this;
    }

    public override Boolean TryGetPlayerAction(out PlayerAction playerAction)
    {
      playerAction = null;
      if (this.Actions.Count == 0)
      {
        return false;
      }

      var actionType = this.Actions.Dequeue();
      if (actionType == PlayerActionTypes.EndTurn)
      {
        return false;
      }

      if (actionType == PlayerActionTypes.TradeWithBank)
      {
        var tradeWithBankInstruction = this.tradeWithBankInstructions.Dequeue();
        
        playerAction = new TradeWithBankAction(
          tradeWithBankInstruction.GivingType,
          tradeWithBankInstruction.ReceivingType,
          tradeWithBankInstruction.ReceivingCount);
      }
      else
      {
        playerAction = new PlayerAction(actionType);
      }

      return true;
    }

    private List<DevelopmentCardTypes> CreateListOfDisplayedDevelopmentCards()
    {
      // TODO: Use Jabberwocky
      if (this.DisplayedDevelopmentCards == null || this.DisplayedDevelopmentCards.Count == 0)
      {
        return null;
      }

      return new List<DevelopmentCardTypes>(this.DisplayedDevelopmentCards);
    }
    #endregion
  }

  public struct PlayKnightInstruction
  {
    public UInt32 RobberHex;
    public Guid RobbedPlayerId;
  }

  public struct PlayMonopolyCardInstruction
  {
    public ResourceTypes ResourceType;
  }

  public struct PlayYearOfPlentyCardInstruction
  {
    public ResourceTypes FirstResourceChoice;
    public ResourceTypes SecondResourceChoice;
  }

  public struct TradeWithBankInstruction
  {
    public ResourceTypes GivingType;
    public ResourceTypes ReceivingType;
    public Int32 ReceivingCount;
  }

  public struct BuildRoadSegmentInstruction
  {
    public UInt32[] Locations;
  }
  
  public struct BuildSettlementInstruction
  {
    public UInt32 Location;
  }

  public struct BuildCityInstruction
  {
    public UInt32 Location;
  }
}
