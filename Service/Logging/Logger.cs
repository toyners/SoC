
namespace Jabberwocky.SoC.Service.Logging
{
  using System;

  public static class Logger
  {
    #region Events
    public static Action<String> MessageEvent;

    public static Action<String> ExceptionEvent;
    #endregion

    #region Methods
    public static void Exception(String message)
    {
      ExceptionEvent?.Invoke(message);
    }

    public static void Message(String message)
    {
      MessageEvent?.Invoke(message);
    }
    #endregion
  }
}
