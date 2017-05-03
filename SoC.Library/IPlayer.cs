
namespace Jabberwocky.SoC.Library
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

  public interface IGameConnector
  {
    Boolean RequestConnectionToGame(out Guid gameId);
  }
}
