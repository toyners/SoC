
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

  [DataContract]
  public class PlayerData
  {
    [DataMember]
    public String Username { get; private set; }

    [DataMember]
    public UInt32 GamesPlayed { get; private set; }

    public PlayerData(String username, UInt32 gamesPlayed)
    {
      // Check null or empty
      this.Username = username;
      this.GamesPlayed = gamesPlayed;
    }
  }
}
