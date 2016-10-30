
namespace Jabberwocky.SoC.Service.Logging
{
  using System;

  public interface ILog
  {
    void Message(String message);

    void Exception(String message);
  }
}
