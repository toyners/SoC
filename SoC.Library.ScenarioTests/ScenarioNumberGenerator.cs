using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioNumberGenerator : INumberGenerator
    {
        private readonly Queue<Tuple<uint, uint>> diceRolls = new Queue<Tuple<uint, uint>>();
        private readonly List<int> numbers = new List<int>();

        public void AddRandomNumber(int number)
        {
            this.numbers.Add(number);
        }

        public void AddTwoDiceRoll(uint dice1, uint dice2)
        {
            this.diceRolls.Enqueue(new Tuple<uint, uint>(dice1, dice2));
        }

        public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
        {
            var number = this.numbers[0];
            this.numbers.RemoveAt(0);
            return number;
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