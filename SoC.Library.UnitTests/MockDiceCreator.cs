
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
    public MockDiceCreator AddExplicitSequence(UInt32[] diceRolls)
    {
      this.diceRollSequences.Add(diceRolls);
      return this;
    }

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

    public MockDice Create()
    {
      return new MockDice(diceRollSequences);
    }
    #endregion
  }
}
