
namespace Jabberwocky.SoC.Library.Storage
{
  public interface IGameDataReader<SectionKey, Key, Enum>
  {
    IGameDataSection<SectionKey, Key, Enum> GetSection(SectionKey sectionKey);
  }
}
