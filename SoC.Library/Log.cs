
namespace Jabberwocky.SoC.Library
{
    using System.Collections.Generic;
    using System.IO;
    using Jabberwocky.SoC.Library.Interfaces;

    public class Log : ILog
    {
        public List<string> Messages { get; private set; } = new List<string>();

        public void Add(string message) => this.Messages.Add(message);

        public void WriteToFile(string filePath) => File.WriteAllLines(filePath, this.Messages);
    }
}
