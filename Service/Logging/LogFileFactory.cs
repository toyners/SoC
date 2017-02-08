
namespace Jabberwocky.SoC.Service.Logging
{
  using System;
  using Toolkit.IO;
  using Toolkit.Logging;
  using Toolkit.Path;
  using Toolkit.String;

  public class LogFileFactory : ILoggerFactory
  {
    private String basePath;

    public LogFileFactory(String basePath)
    {
      basePath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'basePath' is null or empty.");
      this.basePath = PathOperations.CompleteDirectoryPath(basePath);
      DirectoryOperations.EnsureDirectoryExists(this.basePath);
    }

    public ILogger Create(String fileName)
    {
      var filePath = this.basePath + fileName;
      return new Logger(filePath);
    }
  }
}
