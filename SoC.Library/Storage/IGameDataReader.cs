
namespace Jabberwocky.SoC.Library.Storage
{
  public interface IGameDataReader
  {
    GameDataSection Section(GameDataSectionKeys key);
  }
}
