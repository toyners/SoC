
namespace Jabberwocky.SoC.Library
{
  using System.Collections.Generic;
  using Interfaces;

  public static class PlayerTurnOrderCreator
  {
    public static IPlayer[] Create(IPlayer[] players, INumberGenerator dice)
    {
      // Roll dice for each player
      var rollsByPlayer = new Dictionary<uint, uint>();
      var rolls = new List<uint>(players.Length);
      uint index = 0;
      for (; index < players.Length; index++)
      {
        uint roll;
        do
        {
          dice.RollTwoDice(out var dice1, out var dice2);
          roll = dice1 + dice2;
        } while (rolls.Contains(roll));
        
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
