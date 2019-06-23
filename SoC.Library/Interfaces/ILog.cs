
namespace Jabberwocky.SoC.Library.Interfaces
{
    public interface ILog
    {
        void Add(string message);
        void WriteToFile(string filePath);
    }
}
