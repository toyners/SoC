
namespace Jabberwocky.SoC.Service
{
  using System;

  public interface IPlayerCardRepository
  {
    PlayerData GetPlayerData(String userName);
  }
}
