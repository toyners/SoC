
namespace Jabberwocky.SoC.Library
{
  using System;
  using Enums;
  using Interfaces;

  public class GameControllerFactory
  {
    public IGameController Create(GameFilter gameOptions)
    {
      if (gameOptions.Connection == GameConnectionTypes.Local)
      {
        return new LocalGameController();
      }

      throw new NotImplementedException();
    }
  }
}
