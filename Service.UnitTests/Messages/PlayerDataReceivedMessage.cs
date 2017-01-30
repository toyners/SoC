
namespace Service.UnitTests.Messages
{
  using System;
  using Jabberwocky.SoC.Service;

  public class PlayerDataReceivedMessage : MessageBase
  {
    #region Construction
    public PlayerDataReceivedMessage(PlayerData playerData)
    {
      this.PlayerData = playerData;
    }
    #endregion

    #region Properties
    public PlayerData PlayerData { get; private set; }
    #endregion

    #region Methods
    public override Boolean IsSameAs(MessageBase messageBase)
    {
      if (!base.IsSameAs(messageBase))
      {
        return false;
      }

      var playerDataReceivedMessage = (PlayerDataReceivedMessage)messageBase;
      return this.PlayerData.IsAnonymous == playerDataReceivedMessage.PlayerData.IsAnonymous &&
        this.PlayerData.Username == playerDataReceivedMessage.PlayerData.Username;
    }

    public override String ToString()
    {
      return String.Format("{0}, IsAnonymous: {1}, Username: {2}", this.GetType(), this.PlayerData.IsAnonymous, this.PlayerData.Username);
    }
    #endregion
  }
}
