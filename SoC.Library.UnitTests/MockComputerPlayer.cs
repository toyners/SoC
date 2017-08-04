
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  /// <summary>
  /// Used to set opponent player behaviour for testing purposes
  /// </summary>
  public class MockPlayer : IComputerPlayer
  {
    #region Fields
    public UInt32 HiddenDevelopmentCards;
    public UInt32 ResourceCards;
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards;
    public readonly String Name;

    public Queue<UInt32> SettlementLocations;
    public Queue<Road> Roads;
    #endregion

    #region Construction
    public MockPlayer()
    {
      this.Id = Guid.NewGuid();
    }

    public MockPlayer(String name) : this()
    {
      this.Name = name;
    }
    #endregion

    #region Properties
    public Guid Id { get; private set; }
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
}
