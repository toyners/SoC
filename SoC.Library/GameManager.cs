
namespace Jabberwocky.SoC.Library
{
  using System;

  public class GameManager : IGameManager
  {
    public GameManager(Board board, UInt32 playerCount, IDiceRoller diceRoller, DevelopmentCardPile cardPile)
    {

    }

    public Board Board
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public UInt32[] GetFirstSetupPassOrder()
    {
      throw new NotImplementedException();
    }

    public UInt32[] GetSecondSetupPassOrder()
    {
      throw new NotImplementedException();
    }

    public void PlaceRoad(UInt32 playerId)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(UInt32 position)
    {
      throw new NotImplementedException();
    }
  }
}
