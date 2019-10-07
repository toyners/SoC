
namespace Jabberwocky.SoC.Library
{
    using System;
    using Jabberwocky.SoC.Library.Interfaces;

    public class NumberGenerator : INumberGenerator
    {
        private readonly Random random = new Random();

        public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
        {
            return this.random.Next(exclusiveMaximum);
        }

        public void RollTwoDice(out uint dice1, out uint dice2)
        {
            dice1 = (uint)this.random.Next(6) + 1;
            dice2 = (uint)this.random.Next(6) + 1;
        }
    }
}
