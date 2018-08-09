
namespace Jabberwocky.SoC.Library.Storage
{
  public interface IGameDataReader
  {
    GameDataSection GetSection(GameDataSectionKeys key);
  }
}
