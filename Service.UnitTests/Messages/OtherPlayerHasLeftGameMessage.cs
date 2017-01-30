
namespace Service.UnitTests.Messages
{
  using System;

  public class OtherPlayerHasLeftGameMessage : MessageBase
  {
    public OtherPlayerHasLeftGameMessage(String userName)
    {
      this.MessageText = userName + " has left the game.";
    }
  }
}
