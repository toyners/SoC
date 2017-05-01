
namespace Jabberwocky.SoC.Library
{
  public class GameManagerFactory : IGameManagerFactory
  {
    public IGameManager Create()
    {
      var board = new Board(BoardSizes.Standard);
      return new GameManager(board, 1, new DiceRoller(), new DevelopmentCardPile());
    }
  }
}
