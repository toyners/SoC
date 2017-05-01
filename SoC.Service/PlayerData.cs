
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Runtime.Serialization;

  [DataContract]
  public class PlayerData
  {
    #region Construction
    public PlayerData()
    {
      this.IsAnonymous = true;
    }

    public PlayerData(String username)
    {
      // Check null or empty
      this.Username = username;
    }
    #endregion

    #region Properties
    [DataMember]
    public Boolean IsAnonymous { get; private set; }

    [DataMember]
    public String Username { get; private set; }
    #endregion
  }
}
