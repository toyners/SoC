

namespace Jabberwocky.SoC.Library
{
    public class GameServerTimer : IGameTimer
    {
        private int counter = 40;

        public bool IsLate { get { return --this.counter == 0; } }

        public void Reset()
        {
            this.counter = 40;
        }
    }
}
