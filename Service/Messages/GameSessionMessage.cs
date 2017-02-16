
namespace Jabberwocky.SoC.Service.Messages
{
  using System;

  internal class GameSessionMessage
  {
    [Flags]
    public enum Types
    {
      AddPlayer = 1,
      ConfirmGameInitialized = 2,
      LaunchGame = 4,
      Personal = 8,
      RemovePlayer = 16,
      RequestTownPlacement = 32,
    }

    public readonly Types Type;

    public readonly IClientCallback Client;

    public GameSessionMessage(Types type, IClientCallback client)
    {
      this.Type = type;
      this.Client = client;
    }
  }
}
