
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library
{
  public class GameSessionManager : IGameSessionManager
  {
    public IGameSession Create()
    {
      var board = new Board(BoardSizes.Standard);
      return new GameSession(board, 1, new DiceRoller(), new DevelopmentCardPile());
    }
  }
}
