
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using GameBoards;

  /// <summary>
  /// Used to set computer player behaviour for testing purposes
  /// </summary>
  public class MockComputerPlayer : IComputerPlayer
  {
    //private Queue<UInt32> 
    public MockComputerPlayer(UInt32 firstSettlementLocation, Road firstRoad, UInt32 secondSettlementLocation, Road secondRoad)
    {

    }

    public Guid Id
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Road ChooseRoad(GameBoardData gameBoardData)
    {
      throw new NotImplementedException();
    }

    public UInt32 ChooseSettlementLocation(GameBoardData gameBoardData)
    {
      throw new NotImplementedException();
    }

    public void DropResources(Int32 resourceCount)
    {
      throw new NotImplementedException();
    }

    public PlayerDataView GetDataView()
    {
      throw new NotImplementedException();
    }
  }
}
