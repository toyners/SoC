
namespace Jabberwocky.SoC.Library
{
  using Enums;

  public class GameOptions
  {
    public GameConnectionTypes Connection;

    public uint MaxPlayers;

    public uint MaxAIPlayers;

    public GameOptions()
    {
      this.Connection = GameConnectionTypes.Local;
      this.MaxPlayers = 1;
      this.MaxAIPlayers = 3;
    }
  }
}
