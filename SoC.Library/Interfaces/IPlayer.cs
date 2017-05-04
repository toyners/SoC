
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public interface IPlayer
  {
    void RequestConnectionToGame();

    void SetGameManager(IGameManager gameManager);
  }
}
