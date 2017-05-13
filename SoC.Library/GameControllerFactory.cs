
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Enums;
  using Interfaces;

  public class GameControllerFactory
  {
    public IGameController Create(GameConnectionTypes connectionType)
    {
      if (connectionType == GameConnectionTypes.Local)
      {
        return new LocalGameController();
      }
      else
      {
        throw new NotImplementedException();
      }
    }
  }
}
