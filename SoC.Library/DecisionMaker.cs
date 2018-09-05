
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.SoC.Library.Interfaces;

  public class DecisionMaker
  {
    private readonly INumberGenerator numberGenerator;
    private readonly List<Action> decisionTable = new List<Action>();

    public DecisionMaker(INumberGenerator numberGenerator)
    {
      this.numberGenerator = numberGenerator;
    }

    public void Reset()
    {
      this.decisionTable.Clear();
    }

    public void AddDecision(Action action, uint multiplier = 1)
    {
      if (this.decisionTable.Count + multiplier > this.decisionTable.Capacity)
      {
        this.decisionTable.Capacity = this.decisionTable.Count + (int)multiplier;
      }

      for (var count = multiplier; count > 0; count--)
      {
        this.decisionTable.Add(action);
      }
    }

    public Action DetermineDecision()
    {
      var n = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(this.decisionTable.Count);
      return this.decisionTable[n];
    }
  }
}
