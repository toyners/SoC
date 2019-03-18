
namespace Jabberwocky.SoC.Library
{
    public interface ILog
    {
        void Add(string message);
        void WriteToFile(string filePath);
    }
}
