
namespace Jabberwocky.SoC.Service.Messages
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  internal class LaunchGameMessage : GameSessionMessage
  {
    public LaunchGameMessage(IServiceProviderCallback client) : base(Types.LaunchGame, client) { }
  }
}
