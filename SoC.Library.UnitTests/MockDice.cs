using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library.UnitTests
{
  public class MockDice : INumberGenerator
  {
    #region Fields
    private Int32 index;
    private List<UInt32> numbers;
    #endregion

    #region Construction
    public MockDice(UInt32[] first, params UInt32[][] rest)
    {
      this.numbers = new List<UInt32>(first);
      foreach (var sequence in rest)
      {
        this.numbers.AddRange(sequence);
      }
    }

    public MockDice(List<UInt32[]> numbers)
    {
      this.numbers = new List<UInt32>(numbers[0]);
      for (Int32 i = 1; i < numbers.Count; i++)
      {
        this.numbers.AddRange(numbers[i]);
      }
    }
    #endregion

    #region Methods
    public void AddSequence(UInt32[] rolls)
    {
      this.numbers.AddRange(rolls);
    }

    public UInt32 RollTwoDice()
    {
      return this.GetNextNumber();
    }

    public Int32 GetRandomNumberBetweenZeroAndMaximum(Int32 exclusiveMaximum)
    {
      return (Int32)this.GetNextNumber();
    }

    private UInt32 GetNextNumber()
    {
      if (this.index >= this.numbers.Count)
      {
        throw new IndexOutOfRangeException("No more dice rolls.");
      }

      return this.numbers[this.index++];
    }
    #endregion
  }
}
