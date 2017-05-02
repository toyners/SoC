
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameManager_UnitTests
  {
    #region Methods
    [Test]
    public void RegisterPlayer_GameManagerIsEmpty_PlayerIsRegistered()
    {
      var board = new Board(BoardSizes.Standard);
      var diceRoller = new DiceRoller();
      var cardPile = new DevelopmentCardPile();
      IGameManager gameManager = new GameManager(board, 2, diceRoller, cardPile);

      var player = new Player(board);

      var result = gameManager.RegisterPlayer(player);

      result.ShouldBeTrue();
    }

    [Test]
    public void RegisterPlayer_GameManagerNeedsLastPlayer_PlayerIsRegistered()
    {
      var board = new Board(BoardSizes.Standard);
      var diceRoller = new DiceRoller();
      var cardPile = new DevelopmentCardPile();
      IGameManager gameManager = new GameManager(board, 2, diceRoller, cardPile);

      var player1 = new Player(board);
      var player2 = new Player(board);

      gameManager.RegisterPlayer(player1);
      var result = gameManager.RegisterPlayer(player2);

      result.ShouldBeTrue();
    }

    [Test]
    public void RegisterPlayer_GameManagerIsFull_PlayerIsNotRegistered()
    {
      var board = new Board(BoardSizes.Standard);
      var diceRoller = new DiceRoller();
      var cardPile = new DevelopmentCardPile();
      IGameManager gameManager = new GameManager(board, 2, diceRoller, cardPile);

      var player1 = new Player(board);
      var player2 = new Player(board);
      var player3 = new Player(board);

      gameManager.RegisterPlayer(player1);
      gameManager.RegisterPlayer(player2);
      var result = gameManager.RegisterPlayer(player3);

      result.ShouldBeTrue();
    }

    [Test]
    public void GetFirstSetupPassOrder_ReturnsPlayerOrderBasedOnDiceRolls()
    {
      var diceRoller = Substitute.For<IDiceRoller>();
      diceRoller.RollTwoDice().Returns(4u, 8u, 6u, 10u);
      var gameManager = new GameManager(new Board(BoardSizes.Standard), 4, diceRoller, new DevelopmentCardPile());


      gameManager.GetFirstSetupPassOrder().ShouldBe(new [] { 3u, 1u, 2u, 0u });
    }

    [Test]
    public void GetFirstSetupPassOrder_SameRollForTwoPlayersCausesReroll_ReturnsPlayerOrderBasedOnDiceRolls()
    {
      var diceRoller = Substitute.For<IDiceRoller>();
      diceRoller.RollTwoDice().Returns(10u, 8u, 6u, 10u, 12u);
      var gameManager = new GameManager(new Board(BoardSizes.Standard), 4, diceRoller, new DevelopmentCardPile());


      gameManager.GetFirstSetupPassOrder().ShouldBe(new[] { 3u, 0u, 1u, 2u });
    }

    [Test]
    public void GetFirstSetupPassOrder_SameRollCausesTwoRoundsOfRerolls_ReturnsPlayerOrderBasedOnDiceRolls()
    {
      var diceRoller = Substitute.For<IDiceRoller>();
      diceRoller.RollTwoDice().Returns(10u, 10u, 10u, 10u, 7u, 6u, 7u, 8u);
      var gameManager = new GameManager(new Board(BoardSizes.Standard), 4, diceRoller, new DevelopmentCardPile());


      gameManager.GetFirstSetupPassOrder().ShouldBe(new[] { 0u, 3u, 1u, 2u });
    }
    #endregion 
  }
}
