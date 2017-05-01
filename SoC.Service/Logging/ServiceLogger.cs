
namespace Jabberwocky.SoC.Service.Logging
{
  using System;
  using System.IO;

  public static class ServiceLogger
  {
    private static StreamWriter writer;

    #region Methods
    public static void Exception(String message)
    {
      EnsureWriterIsOpen();
      writer.WriteLine(message);
      writer.Flush();
    }

    public static void Message(String message)
    {
      EnsureWriterIsOpen();
      writer.WriteLine(message);
      writer.Flush();
    }

    private static void EnsureWriterIsOpen()
    {
      if (writer == null)
      {
        writer = new StreamWriter(@"C:\projects\soc.log", true);
        writer.WriteLine("Logging started.");
        writer.Flush();
      }
    }
    #endregion
  }
}
