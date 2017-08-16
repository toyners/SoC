
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using GameBoards;
  using System.Collections.Generic;

  /// <summary>
  /// Used to set opponent player behaviour for testing purposes
  /// </summary>
  public class MockComputerPlayer : MockPlayer, IComputerPlayer
  {
    #region Fields
    public UInt32 HiddenDevelopmentCards;
    public UInt32 ResourceCards;
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards;

    public Queue<UInt32> SettlementLocations;
    public Queue<Road> Roads;
    //public ResourceBag Resources;
    public ResourceClutch DroppedResources;
    #endregion

    #region Construction
    public MockComputerPlayer(String name) : base(name) {}
    #endregion

    #region Methods
    public ResourceClutch ChooseResourcesToDrop()
    {
      return DroppedResources;
    }

    public Road ChooseRoad(GameBoardData gameBoardData)
    {
      return this.Roads.Dequeue();
    }

    public UInt32 ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      return this.SettlementLocations.Dequeue();
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
