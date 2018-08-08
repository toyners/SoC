
namespace Jabberwocky.SoC.Library.Storage
{
  using System;

  public interface IGameDataReader
  {
    GameDataSection Section(String key);
  }
}
