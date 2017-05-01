
namespace Service.UnitTests.Messages
{
  using System;
  using Jabberwocky.SoC.Service;

  public class InitializeGameMessage : MessageBase
  {
    #region Construction
    public InitializeGameMessage(GameInitializationData gameData)
    {
      this.GameData = gameData;
    }
    #endregion
    
    #region Properties
    public GameInitializationData GameData { get; private set; }
    #endregion

    #region Methods
    public override Boolean IsSameAs(MessageBase messageBase)
    {
      if (!base.IsSameAs(messageBase))
      {
        return false;
      }

      var initializeGameMessage = (InitializeGameMessage)messageBase;

      if (this.GameData == null || initializeGameMessage.GameData == null || this.GameData.BoardData.Length != initializeGameMessage.GameData.BoardData.Length)
      {
        return false;
      }

      for (int i = 0; i < this.GameData.BoardData.Length; i++)
      {
        if (this.GameData.BoardData[i] != initializeGameMessage.GameData.BoardData[i])
        {
          return false;
        }
      }

      return true;
    }
    #endregion
  }
}
