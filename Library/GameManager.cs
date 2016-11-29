
namespace Jabberwocky.SoC.Library
{
  using System;
  using Service;

  public class GameManager
  {
    #region Fields
    private Board board;

    private IDiceRoller diceRoller;

    private Player[] players;

    private DevelopmentCardPile cardPile;
    #endregion

    #region Construction
    public GameManager(Board board, IDiceRoller diceRoller, UInt32 playerCount, DevelopmentCardPile cardPile)
    {
      this.board = board;
      this.diceRoller = diceRoller;
      this.players = new Player[playerCount];
      this.cardPile = cardPile;
    }
    #endregion

    #region Methods
    public void Start()
    {
      var setupOrder = this.DetermineSetupOrder(this.players);

      foreach (var player in setupOrder)
      {
        player.CompleteSetup();
      }

      var playingOrder = this.DeterminePlayingOrder(this.players);

      while (true)
      {
        foreach (var player in this.players)
        {
          player.CompleteTurn();
        }
      }
    }

    private Player[] DeterminePlayingOrder(Player[] players)
    {
      throw new NotImplementedException();
    }

    private Player[] DetermineSetupOrder(Player[] players)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
