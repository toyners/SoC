
namespace Jabberwocky.SoC.Service.Logging
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class Logger : ILog
  {
    public Action<String> MessageEvent;

    public void Exception(String message)
    {
      throw new NotImplementedException();
    }

    public void Message(String message)
    {
      MessageEvent?.Invoke(message);
    }
  }
}
