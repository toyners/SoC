

namespace Jabberwocky.SoC.Library
{
    public interface IGameTimer
    {
        void Reset();
        bool IsLate { get; }
    }
}
