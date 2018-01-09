
namespace Jabberwocky.SoC.Library
{
  using System;
  using GameBoards;
  using Interfaces;

  public class GameSession : IGameSession
  {
    public GameSession(GameBoardManager board, UInt32 playerCount, INumberGenerator diceRoller, Object cardPile)
    {
      if (playerCount < 2 || playerCount > 4)
      {
        throw new ArgumentOutOfRangeException(String.Format("Maximum Player count must be within range 2-4 inclusive. Was {0}.", playerCount), (Exception)null);
      }
    }

    public GameSession()
    {

    }

    public GameBoardManager Board
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

    public Boolean RegisterPlayer(ClientAccount clientAccount)
    {
      throw new NotImplementedException();
    }
  }
}
