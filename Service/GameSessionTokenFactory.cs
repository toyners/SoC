
namespace Jabberwocky.SoC.Service
{
  using System;

  public class GameSessionTokenFactory : IGameSessionTokenFactory
  {
    public Guid CreateGameSessionToken()
    {
      return Guid.NewGuid();
    }
  }
}
