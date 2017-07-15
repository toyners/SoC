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

    public MockDice(List<UInt32[]> diceRolls)
    {
      var rolls = new List<UInt32>(diceRolls[0]);
      for (Int32 i = 1; i < diceRolls.Count; i++)
      {
        rolls.AddRange(diceRolls[i]);
      }

      this.diceRolls = rolls.ToArray();
    }

    public bool ReturnRandomRollOnException { get; set; }

    public UInt32 RollTwoDice()
    {
      if (this.index >= this.diceRolls.Length)
      {
        if (this.ReturnRandomRollOnException)
        {
          var random = new Random();
          return (UInt32)random.Next(2, 13);
        }

        throw new IndexOutOfRangeException("No more dice rolls.");
      }

      return this.diceRolls[this.index++];
    }
  }
}
