
namespace SoC.Harness
{
  using System.Collections.Generic;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Interfaces;

  public class NumberGenerator : INumberGenerator
  {
    private Dice dice;
    private Queue<uint> diceRolls;

    public NumberGenerator()
    {
      this.diceRolls = new Queue<uint>(new uint[] { 12, 6, 4, 3 });
      this.dice = new Dice();
    }

    public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
    {
      return this.dice.GetRandomNumberBetweenZeroAndMaximum(exclusiveMaximum);
    }

    public uint RollTwoDice()
    {
      if (this.diceRolls.Count > 0)
      {
        return this.diceRolls.Dequeue();
      }

      return this.dice.RollTwoDice();
    }
  }
}
