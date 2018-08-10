
namespace Jabberwocky.SoC.Library.Storage
{
  using System;

  public class MemoryXmlDataReader : IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes>
  {
    public IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> GetSection(GameDataSectionKeys sectionKey)
    {
      return new XmlDataSection();
    }
  }
}
