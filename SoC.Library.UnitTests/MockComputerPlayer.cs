
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
    private Queue<PlayKnightAction> playKnightCardActions = new Queue<PlayKnightAction>();
    private Queue<PlayMonopolyCardAction> playMonopolyCardActions = new Queue<PlayMonopolyCardAction>();
    public Queue<UInt32> SettlementLocations;
    public Queue<Tuple<UInt32, UInt32>> Roads = new Queue<Tuple<UInt32, UInt32>>();
    public Queue<Tuple<UInt32, UInt32>> InitialInfrastructure = new Queue<Tuple<UInt32, UInt32>>();
    public Queue<PlayerAction> Actions = new Queue<PlayerAction>();
    public ResourceClutch DroppedResources;
    #endregion

    #region Construction
    public MockComputerPlayer(String name) : base(name) { }
    #endregion

    #region Methods
    public void AddInitialInfrastructureChoices(UInt32 firstSettlmentLocation, UInt32 firstRoadEndLocation, UInt32 secondSettlementLocation, UInt32 secondRoadEndLocation)
    {
      this.InitialInfrastructure.Enqueue(new Tuple<UInt32, UInt32>(firstSettlmentLocation, firstRoadEndLocation));
      this.InitialInfrastructure.Enqueue(new Tuple<UInt32, UInt32>(secondSettlementLocation, secondRoadEndLocation));
    }

    public void AddCityChoices(UInt32[] cityChoices)
    {
      foreach (var cityChoice in cityChoices)
      {
        this.CityLocations.Enqueue(cityChoice);
        this.Actions.Enqueue(PlayerAction.BuildCity);
      }
    }

    public void AddRoadChoices(UInt32[] roadChoices)
    {
      for (var index = 0; index < roadChoices.Length; index += 2)
      {
        this.Roads.Enqueue(new Tuple<UInt32, UInt32>(roadChoices[index], roadChoices[index + 1]));
        this.Actions.Enqueue(PlayerAction.BuildRoad);
      }
    }

    public MockComputerPlayer AddBuyDevelopmentCardChoice(UInt32 count)
    {
      for (; count > 0; count--)
      {
        this.Actions.Enqueue(PlayerAction.BuyDevelopmentCard);
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

    public MockComputerPlayer AddPlaceMonopolyCardAction(PlayMonopolyCardAction playMonopolyCardAction)
    {
      this.playMonopolyCardActions.Enqueue(playMonopolyCardAction);
      this.Actions.Enqueue(PlayerAction.PlayMonopolyCard);
      return this;
    }

    public MockComputerPlayer AddPlaceKnightCard(PlayKnightAction playKnightCardAction)
    {
      this.playKnightCardActions.Enqueue(playKnightCardAction);
      this.Actions.Enqueue(PlayerAction.PlayKnightCard);
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

    public override ResourceClutch ChooseResourcesToDrop()
    {
      return DroppedResources;
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

    public MockComputerPlayer EndTurn()
    {
      this.Actions.Enqueue(PlayerAction.EndTurn);
      return this;
    }

    public override PlayerAction GetPlayerAction()
    {
      if (this.Actions.Count == 0)
      {
        return PlayerAction.EndTurn;
      }

      var playerAction = this.Actions.Dequeue();
      return playerAction;
    }

    public void RemoveAllResources()
    {
      var resources = new ResourceClutch(this.BrickCount, this.GrainCount, this.LumberCount, this.OreCount, this.WoolCount);
      this.RemoveResources(resources);
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

  public struct PlayKnightAction
  {
    public UInt32 RobberHex;
    public Guid RobbedPlayerId;
  }

  public struct PlayMonopolyCardAction
  {
    public ResourceTypes MonopolyType;
  }
}
