
namespace Jabberwocky.SoC.Service
{
  using System;

  public interface IGameSessionTokenFactory
  {
    Guid CreateGameSessionToken();
  }
}
