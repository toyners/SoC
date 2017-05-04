
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public interface IGameConnector
  {
    Boolean RequestConnectionToGame(out Guid gameId);
  }
}
