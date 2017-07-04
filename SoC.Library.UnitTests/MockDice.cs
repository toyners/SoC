using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library.UnitTests
{
  public class MockDice : IDice
  {
    private Int32 index;
    private UInt32[] diceRolls;

    public MockDice(UInt32[] first, params UInt32[][] rest)
    {
      var rolls = new List<UInt32>(first);
      foreach (var sequence in rest)
      {
        rolls.AddRange(sequence);
      }

      this.diceRolls = rolls.ToArray();
    }

    public UInt32 RollTwoDice()
    {
      if (this.index >= this.diceRolls.Length)
      {
        throw new IndexOutOfRangeException("No more dice rolls.");
      }

      return this.diceRolls[this.index++];
    }
  }
}
