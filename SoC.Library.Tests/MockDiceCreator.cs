
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;

  public class MockDiceCreator
  {
    #region Fields
    private List<UInt32[]> diceRollSequences = new List<UInt32[]>();
    #endregion

    #region Methods
    public MockDiceCreator AddExplictDiceRoll(UInt32 diceRoll)
    {
      this.diceRollSequences.Add(new[] { diceRoll });
      return this;
    }

    public MockDiceCreator AddExplicitDiceRollSequence(UInt32[] diceRolls)
    {
      this.diceRollSequences.Add(diceRolls);
      return this;
    }

    // Add a random sequence of rolls. 
    public MockDiceCreator AddRandomSequence(Int32 diceRollCount)
    {
      var diceRolls = new UInt32[diceRollCount];

      var random = new Random();
      for (Int32 i = 0; i < diceRollCount; i++)
      {
        diceRolls[i] = (UInt32)random.Next(2, 13);
      }

      this.diceRollSequences.Add(diceRolls);
      return this;
    }

    // Add a random sequence of distinct rolls. Can't have more than 11 dice rolls
    public MockDiceCreator AddRandomSequenceWithNoDuplicates(Int32 diceRollCount)
    {
      if (diceRollCount > 11)
      {
        throw new Exception("Can't have more than 11 dictinct rolls");
      }

      var diceRolls = new UInt32[diceRollCount];
      var currentRolls = new HashSet<UInt32>();

      var random = new Random();
      for (Int32 i = 0; i < diceRollCount; i++)
      {
        UInt32 roll = 2u;
        do
        {
          roll = (UInt32)random.Next(2, 13);
        }
        while (currentRolls.Contains(roll));

        currentRolls.Add(roll);
        diceRolls[i] = roll;
      }

      this.diceRollSequences.Add(diceRolls);
      return this;
    }

    public MockDice Create()
    {
      var mockDice = new MockDice(diceRollSequences);
      return mockDice;
    }
    #endregion
  }
}
