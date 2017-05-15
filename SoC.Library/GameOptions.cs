
namespace Jabberwocky.SoC.Library
{
  using System;
  using Enums;

  public class GameOptions
  {
    public GameConnectionTypes Connection;

    public UInt32 MaxPlayers;

    public UInt32 MaxAIPlayers;

    public GameOptions()
    {
      this.Connection = GameConnectionTypes.Local;
      this.MaxPlayers = 1;
      this.MaxAIPlayers = 3;
    }
  }
}
