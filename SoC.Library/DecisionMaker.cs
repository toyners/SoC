
namespace Jabberwocky.SoC.Library
{
  using System.Collections.Generic;
  using Jabberwocky.SoC.Library.Interfaces;

  public class DecisionMaker
  {
    private INumberGenerator numberGenerator;
    private List<int> decisionTable = new List<int>();

    public DecisionMaker(INumberGenerator numberGenerator)
    {
      this.numberGenerator = numberGenerator;
    }

    public void Reset()
    {
      this.decisionTable.Clear();
    }

    public void AddDecision(int id, uint multiplier = 1)
    {
      if (this.decisionTable.Count + multiplier > this.decisionTable.Capacity)
      {
        this.decisionTable.Capacity = this.decisionTable.Count + (int)multiplier;
      }

      for (var count = multiplier; count > 0; count--)
      {
        this.decisionTable.Add(id);
      }
    }

    public int DetermineDecision()
    {
      var n = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(this.decisionTable.Count);
      return this.decisionTable[n];
    }
  }
}
