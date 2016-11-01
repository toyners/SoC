
namespace Jabberwocky.SoC.Service.Logging
{
  using System;

  public static class Logger
  {
    #region Methods
    public static void Exception(String message)
    {
    }

    public static void Message(String message)
    {
      Console.WriteLine(message);
    }
    #endregion
  }
}
