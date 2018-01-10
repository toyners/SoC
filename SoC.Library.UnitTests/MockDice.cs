using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library.UnitTests
{
  public class MockDice : INumberGenerator
  {
    #region Fields
    private Int32 index;
    private List<UInt32> diceRolls;
    #endregion

    #region Construction
    public MockDice(UInt32[] first, params UInt32[][] rest)
    {
      this.diceRolls = new List<UInt32>(first);
      foreach (var sequence in rest)
      {
        this.diceRolls.AddRange(sequence);
      }
    }

    public MockDice(List<UInt32[]> rolls)
    {
      this.diceRolls = new List<UInt32>(rolls[0]);
      for (Int32 i = 1; i < rolls.Count; i++)
      {
        this.diceRolls.AddRange(rolls[i]);
      }
    }
    #endregion

    #region Methods
    public void AddSequence(UInt32[] rolls)
    {
      this.diceRolls.AddRange(rolls);
    }

    public UInt32 RollTwoDice()
    {
      if (this.index >= this.diceRolls.Count)
      {
        throw new IndexOutOfRangeException("No more dice rolls.");
      }

      return this.diceRolls[this.index++];
    }

    public Int32 GetRandomNumberBetweenZeroAndMaximum(UInt16 exclusiveMaximum)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
