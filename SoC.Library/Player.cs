
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Interfaces;

  public class Player : IPlayer
  {
    List<Location> Settlements;
    List<DevelopmentCard> Cards;

    UInt32 OreCount;
    UInt32 WheatCount;
    UInt32 SheepCount;
    UInt32 LumberCount;
    UInt32 BrickCount;

    private Board board;
    private IGameController gameConnector;

    public Player(Board board, IGameController gameConnector)
    {
      this.gameConnector = gameConnector;
      this.board = board;
    }

    #region Methods
    public UInt32 GetVictoryPoints() { throw new NotImplementedException(); }

    public void BuildSettlement() { }

    public void UpgradeCity() { }

    public void CompleteSetup()
    {
      //this.board.PlaceStartingSettlement();

      //this.board.PlaceStartingRoad();
      throw new NotImplementedException();
    }

    public void CompleteTurn()
    {
      throw new NotImplementedException();
    }

    public void ProduceResources()
    { }

    public void SetGameManager(IGameManager gameManager)
    {
      throw new NotImplementedException();
    }

    public void RequestConnectionToGame()
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
