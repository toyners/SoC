
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
    public Queue<UInt32> SettlementLocations;
    public Queue<Tuple<UInt32, UInt32>> Roads = new Queue<Tuple<UInt32, UInt32>>();
    public Queue<Tuple<UInt32, UInt32>> InitialInfrastructure = new Queue<Tuple<UInt32, UInt32>>();
    public Queue<PlayerAction> Actions = new Queue<PlayerAction>();
    public ResourceClutch DroppedResources;
    #endregion

    #region Construction
    public MockComputerPlayer(String name) : base(name) {}
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

    public override ResourceClutch ChooseResourcesToDrop()
    {
      return DroppedResources;
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

    public override PlayerAction GetPlayerAction()
    {
      if (this.Actions.Count == 0)
      {
        return PlayerAction.EndTurn;
      }

      var playerAction = this.Actions.Dequeue();
      return playerAction;
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
}
