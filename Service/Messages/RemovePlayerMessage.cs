
namespace Jabberwocky.SoC.Service.Messages
{
  internal class RemovePlayerMessage : GameSessionMessage
  {
    public RemovePlayerMessage(IServiceProviderCallback client) : base(Types.RemovePlayer, client)
    {
    }
  }
}
