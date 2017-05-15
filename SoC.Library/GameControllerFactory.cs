
namespace Jabberwocky.SoC.Library
{
  using System;
  using Enums;
  using Interfaces;

  public class GameControllerFactory
  {
    public IGameController Create(GameOptions gameOptions)
    {
      if (gameOptions.Connection == GameConnectionTypes.Local)
      {
        return new LocalGameController();
      }

      throw new NotImplementedException();
    }
  }
}
