
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using Interfaces;

  public static class PlayerTurnOrderCreator
  {
    public static IPlayer[] Create(IPlayer[] players, INumberGenerator dice)
    {
      // Roll dice for each player
      var rollsByPlayer = new Dictionary<UInt32, UInt32>();
      var rolls = new List<UInt32>(players.Length);
      UInt32 index = 0;
      for (; index < players.Length; index++)
      {
        UInt32 roll = dice.RollTwoDice();
        while (rolls.Contains(roll))
        {
          roll = dice.RollTwoDice();
        }

        rollsByPlayer.Add(roll, index);
        rolls.Add(roll);
      }

      // Reverse sort the rolls
      rolls.Sort((x, y) => { if (x < y) return 1; if (x > y) return -1; return 0; });

      // Produce order based on descending dice roll order
      IPlayer[] setupOrder = new IPlayer[players.Length];
      index = 0;
      foreach (var roll in rolls)
      {
        setupOrder[index++] = players[rollsByPlayer[roll]];
      }

      return setupOrder;
    }
  }
}
