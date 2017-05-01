
namespace Jabberwocky.SoC.Service.Logging
{
  using System;
  using System.IO;
  using Toolkit.Logging;
  using Toolkit.String;

  public class FileLogger : ILogger
  {
    #region Fields
    private Boolean disposed;
    private StreamWriter messageWriter;
    private StreamWriter exceptionWriter;
    private String exceptionWriterFilePath;
    #endregion

    #region Construction
    public FileLogger(String filePath)
    {
      filePath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'filePath'is null or empty.");
      this.messageWriter = new StreamWriter(filePath);
      this.exceptionWriterFilePath = filePath + ".error";
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
      var exceptionMessage = "EXCEPTION: " + message;
      this.messageWriter.WriteLine(exceptionMessage);
      if (this.exceptionWriter == null)
      {
        this.exceptionWriter = new StreamWriter(this.exceptionWriterFilePath);
      }

      this.exceptionWriter.WriteLine(exceptionMessage);
    }

    public void Message(String message)
    {
      this.Message(message, true);
    }

    public void Message(String message, Boolean lineBreak)
    {
      if (lineBreak)
      {
        this.messageWriter.WriteLine(message);
      }
      else
      {
        this.messageWriter.Write(message);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      if (disposing && this.messageWriter != null)
      {
        if (this.messageWriter != null)
        {
          this.messageWriter.Dispose();
          this.messageWriter = null;
        }
        
        if (this.exceptionWriter != null)
        {
          this.exceptionWriter.Dispose();
          this.exceptionWriter = null;
        }
      }

      this.disposed = true;
    }
    #endregion
  }
}
