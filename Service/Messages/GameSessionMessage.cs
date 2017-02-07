
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
      RequestTownPlacement = 16,
      Any = AddPlayer | ConfirmGameInitialized | Personal | RequestTownPlacement
    }

    public readonly Types Type;

    public readonly IServiceProviderCallback Client;

    public GameSessionMessage(Types type, IServiceProviderCallback client)
    {
      this.Type = type;
      this.Client = client;
    }
  }
}
