
namespace Jabberwocky.SoC.Service.Logging
{
  using System;
  using System.IO;
  using Toolkit.Logging;

  public class Logger : ILogger
  {
    #region Fields
    private Boolean disposed;
    private StreamWriter sw;
    #endregion

    #region Construction
    public Logger(String filePath)
    {
      this.sw = new StreamWriter(filePath);
    }
    #endregion

    #region Methods
    public void Close()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public void Exception(String message)
    {
      this.sw.WriteLine("EXCEPTION: " + message);
    }

    public void Message(String message)
    {
      this.Message(message, true);
    }

    public void Message(String message, Boolean lineBreak)
    {
      if (lineBreak)
      {
        this.sw.WriteLine(message);
      }
      else
      {
        this.sw.Write(message);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      if (disposing && this.sw != null)
      {
        this.sw.Close();
        this.sw.Dispose();
        this.sw = null;
      }

      this.disposed = true;
    }
    #endregion
  }
}
