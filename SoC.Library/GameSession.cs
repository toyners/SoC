
namespace Jabberwocky.SoC.Library
{
  using System;
  using Interfaces;

  public class GameSession : IGameSession
  {
    public GameSession(Board board, UInt32 playerCount, IDiceRoller diceRoller, DevelopmentCardPile cardPile)
    {
      if (playerCount < 2 || playerCount > 4)
      {
        throw new ArgumentOutOfRangeException(String.Format("Maximum Player count must be within range 2-4 inclusive. Was {0}.", playerCount), (Exception)null);
      }
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

    public Boolean RegisterPlayer(IPlayer player)
    {
      throw new NotImplementedException();
    }
  }
}
