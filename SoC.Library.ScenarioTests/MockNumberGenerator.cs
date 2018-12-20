using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class MockNumberGenerator : INumberGenerator
    {
        private readonly Queue<Tuple<uint, uint>> diceRolls = new Queue<Tuple<uint, uint>>();
        private readonly Queue<int> numbers = new Queue<int>();

        public void AddRandomNumber(int number)
        {
            this.numbers.Enqueue(number);
        }

        public void AddTwoDiceRoll(uint dice1, uint dice2)
        {
            this.diceRolls.Enqueue(new Tuple<uint, uint>(dice1, dice2));
        }

        public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
        {
            return this.numbers.Dequeue();
        }

        public void RollTwoDice(out uint dice1, out uint dice2)
        {
            dice1 = dice2 = 1;
            if (this.diceRolls.Count > 0)
            {
                var tuple = this.diceRolls.Dequeue();
                dice1 = tuple.Item1;
                dice2 = tuple.Item2;
            }
        }
    }
}