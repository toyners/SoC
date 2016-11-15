
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Runtime.Serialization;

  [DataContract]
  public class GameInitializationData
  {
    [DataMember]
    public Byte[] BoardData { get; set; }

    [DataMember]
    public Byte ColumnCount { get; set; }

    [DataMember]
    public Byte FirstColumnCount { get; set; }
  }
}
