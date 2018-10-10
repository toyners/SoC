
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using Interfaces;

  public class OldGameManager
  {
    #region Fields
    private INumberGenerator diceRoller;

    private PlayerData[] players;

    private Object cardPile;
    #endregion

    #region Construction
    public OldGameManager(GameBoardManager board, UInt32 playerCount, INumberGenerator diceRoller, Object cardPile)
    {
      //TODO: Check for null references
      this.Board = board;
      this.players = new PlayerData[playerCount];
      this.diceRoller = diceRoller;
      this.cardPile = cardPile;
    }
    #endregion

    #region Properties
    public GameBoardManager Board { get; private set; }
    #endregion

    #region Methods
    public void Start()
    {
    }

    private PlayerData[] DeterminePlayingOrder(PlayerData[] players)
    {
      throw new NotImplementedException();
    }

    private PlayerData[] DetermineSetupOrder(PlayerData[] players)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the order of the clients for the first setup pass.
    /// </summary>
    /// <returns>Returns array of player indexs in order of setup.</returns>
    public UInt32[] GetFirstSetupPassOrder()
    {
      // Roll dice for each player
      var rollsByPlayer = new Dictionary<UInt32, UInt32>();
      var rolls = new List<UInt32>(this.players.Length);
      UInt32 index = 0;
      for (; index < this.players.Length; index++)
      {
        /*UInt32 roll = this.diceRoller.RollTwoDice();
        while (rolls.Contains(roll))
        {
          roll = this.diceRoller.RollTwoDice();
        }

        rollsByPlayer.Add(roll, index);
        rolls.Add(roll);*/
      }

      // Reverse sort the rolls
      rolls.Sort((x, y) => { if (x < y) return 1; if (x > y) return -1; return 0; });

      // Produce order based on descending dice roll order
      UInt32[] setupPassOrder = new UInt32[this.players.Length];
      index = 0;
      foreach (var roll in rolls)
      {
        setupPassOrder[index++] = rollsByPlayer[roll];
      }

      return setupPassOrder;
    }

    /// <summary>
    /// Gets the order of the clients for the second setup pass.
    /// </summary>
    /// <returns>Returns array of player indexs in order of setup.</returns>
    public UInt32[] GetSecondSetupPassOrder()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Places town at position for the player.
    /// </summary>
    /// <param name="playerId">Player Id of the new town.</param>
    public void PlaceTown(UInt32 playerId)
    {
      
    }

    /// <summary>
    /// Places road between positions for the player.
    /// </summary>
    /// <param name="playerId">Player Id of the new road.</param>
    public void PlaceRoad(UInt32 playerId)
    {
      
    }
    #endregion
  }
}
