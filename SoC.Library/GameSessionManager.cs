
using System;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library
{
  public class GameSessionManager : IGameSessionManager
  {
    public IGameSession Create()
    {
      var board = new GameBoardManager(BoardSizes.Standard);
      return new GameSession(board, 1, new Dice(), new Object());
    }
  }
}
