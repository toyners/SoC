
namespace Jabberwocky.SoC.Service.Messages
{
  using System;

  internal class AddPlayerMessage : GameSessionMessage
  {
    public readonly String Username;

    public AddPlayerMessage(IServiceProviderCallback sender, String username) : base(Types.AddPlayer, sender)
    {
      this.Username = username;
    }
  }
}
