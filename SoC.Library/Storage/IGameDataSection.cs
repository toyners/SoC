
namespace Jabberwocky.SoC.Library.Storage
{
  using System;

  public interface IGameDataSection<SectionKey, Key, EnumType>
  {
    Boolean GetBooleanValue(Key key);
    Guid GetIdentityValue(Key key);
    Int32 GetIntegerValue(Key key);
    Int32[] GetIntegerArrayValue(Key key);
    IGameDataSection<SectionKey, Key, EnumType> this[SectionKey sectionKey] { get; }
    IGameDataSection<SectionKey, Key, EnumType>[] GetSections(SectionKey sectionKey);
    EnumType[] GetEnumArrayValue(Key key);
    String GetStringValue(Key key);
    String GetStringValues(Key key);
  }
}