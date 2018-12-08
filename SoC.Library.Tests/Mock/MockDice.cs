
namespace Jabberwocky.SoC.Library.UnitTests.Mock
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.Interfaces;

    public class MockDice : INumberGenerator
    {
        #region Fields
        private Queue<Tuple<uint, uint>> diceRolls = new Queue<Tuple<uint, uint>>();
        private uint repeatingRollDice1 = 0;
        private uint repeatingRollDice2 = 0;
        #endregion

        #region Construction
        public MockDice(uint[] first, params uint[][] rest)
        {
            this.AddSequence(first);
            foreach (var sequence in rest)
            {
                this.AddSequence(sequence);
            }
        }

        public MockDice(List<uint[]> numberLists)
        {
            foreach (var numberList in numberLists)
            {
                this.AddSequence(numberList);
            }
        }
        #endregion

        #region Methods
        public void AddSequence(uint[] rolls)
        {
            foreach (var roll in rolls)
            {
                if (roll % 2 == 0)
                {
                    this.diceRolls.Enqueue(new Tuple<uint, uint>(roll / 2, roll / 2));
                }
                else
                {
                    this.diceRolls.Enqueue(new Tuple<uint, uint>((roll / 2) + 1, roll / 2));
                }
            }
        }

        public void AddSequenceWithRepeatingRoll(uint[] rolls, uint repeatingRoll)
        {
            if (rolls != null && rolls.Length > 0)
                this.AddSequence(rolls);

            if (repeatingRoll % 2 == 0)
            {
                this.repeatingRollDice1 = repeatingRoll / 2;
                this.repeatingRollDice2 = repeatingRoll / 2;
            }
            else
            {
                this.repeatingRollDice1 = (repeatingRoll / 2) + 1;
                this.repeatingRollDice2 = repeatingRoll / 2;
            }
        }

        public void RollTwoDice(out uint dice1, out uint dice2)
        {
            this.GetNextNumber(out dice1, out dice2);
        }

        public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
        {
            this.GetNextNumber(out var dice1, out var dice2);
            return (int)(dice1 + dice2);
        }

        private void GetNextNumber(out uint dice1, out uint dice2)
        {
            if (this.diceRolls.Count == 0)
            {
                if (this.repeatingRollDice1 == 0 && this.repeatingRollDice2 == 0)
                    throw new IndexOutOfRangeException("No more dice rolls.");

                dice1 = this.repeatingRollDice1;
                dice2 = this.repeatingRollDice2;
            }
            else
            {
                var roll = this.diceRolls.Dequeue();
                dice1 = roll.Item1;
                dice2 = roll.Item2;
            }
        }
        #endregion
    }
}
