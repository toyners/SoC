

namespace Jabberwocky.SoC.Library
{
    public class GameServerTimer : IGameTimer
    {
        private int counter = 0;
        private readonly int total = 0;

        public bool IsLate
        {
            get
            {
                if (this.total == -1)
                    return false;
                return --this.counter == 0;
            }
        }

        public GameServerTimer(int turnTimeInSeconds) => this.total = this.counter = turnTimeInSeconds;

        public void Reset()
        {
            this.counter = this.total;
        }
    }
}
