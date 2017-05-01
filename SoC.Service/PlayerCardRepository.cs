
namespace Jabberwocky.SoC.Service
{
  using System;

  public class PlayerCardRepository : IPlayerCardRepository
  {
    public PlayerData GetPlayerData(String userName)
    {
      return new PlayerData();
    }
  }
}
