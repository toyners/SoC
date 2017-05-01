
namespace Service.UnitTests.Messages
{
  using System;

  public class OtherPlayerHasLeftGameMessage : MessageBase
  {
    public OtherPlayerHasLeftGameMessage(String userName) : base(userName + " has left the game.")
    {
    }
  }
}
