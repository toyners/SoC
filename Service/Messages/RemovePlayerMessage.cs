
namespace Jabberwocky.SoC.Service.Messages
{
  internal class RemovePlayerMessage : GameSessionMessage
  {
    public RemovePlayerMessage(IClientCallback client) : base(Types.RemovePlayer, client)
    {
    }
  }
}
