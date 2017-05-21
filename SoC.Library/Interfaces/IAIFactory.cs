
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public interface IComputerPlayer
  {

  }

  public interface IComputerPlayerFactory
  {
    IComputerPlayer Create();
  }
}
