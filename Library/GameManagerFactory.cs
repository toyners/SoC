
namespace Jabberwocky.SoC.Library
{
  using System;

  public class GameManagerFactory : IGameManagerFactory
  {
    public IGameManager CreateGameManager(UInt32 playerCount, IDiceRollerFactory diceRollerFactory)
    {
      var board = new Board(BoardSizes.Standard);
      return new GameManager(board, playerCount, diceRollerFactory.Create(), new DevelopmentCardPile());
    }
  }
}
