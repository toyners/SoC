
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  /// <summary>
  /// Used to set opponent player behaviour for testing purposes
  /// </summary>
  public class MockComputerPlayer : IComputerPlayer
  {
    #region Fields
    public UInt32 HiddenDevelopmentCards;
    public UInt32 ResourceCards;
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards;

    public Queue<UInt32> SettlementLocations;
    public Queue<Road> Roads;
    public List<ResourceTypes> Resources;
    #endregion

    #region Construction
    public MockComputerPlayer(String name)
    {
      this.Id = Guid.NewGuid();
      this.Name = name;
    }
    #endregion

    #region Properties
    public Guid Id { get; private set; }

    public Int32 ResourceCount
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public String Name { get; set; }
    #endregion

    #region Methods
    public Road ChooseRoad(GameBoardData gameBoardData)
    {
      return this.Roads.Dequeue();
    }

    public UInt32 ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      return this.SettlementLocations.Dequeue();
    }

    public void DropResources(Int32 resourceCount)
    {
      throw new NotImplementedException();
    }

    public PlayerDataView GetDataView()
    {
      return new PlayerDataView
      {
        Id = this.Id,
        Name = this.Name,
        DisplayedDevelopmentCards = this.CreateListOfDisplayedDevelopmentCards(),
        HiddenDevelopmentCards = this.HiddenDevelopmentCards,
        ResourceCards = this.ResourceCards
      };
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

  public class MockPlayer : Player
  {
    public List<ResourceTypes> Resources;

    public MockPlayer(String name) : base(name)
    {
    }
  }
}
