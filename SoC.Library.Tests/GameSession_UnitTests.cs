
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  public class DevelopmentCardPile { }

  [TestFixture]
  public class GameSession_UnitTests
  {
    #region Methods
    [Test]
    public void GameSession_MaxPlayerCountIsLessThanTwo_ThrowsMeaningfulException()
    {
      var board = new GameBoardManager(BoardSizes.Standard);
      var diceRoller = new Dice();
      var cardPile = new DevelopmentCardPile();

      Should.Throw<ArgumentOutOfRangeException>(() => new GameSession(board, 1, diceRoller, cardPile))
        .Message.ShouldBe("Maximum Player count must be within range 2-4 inclusive. Was 1.");
    }

    [Test]
    public void GameSession_MaxPlayerCountIsMoreThanFour_ThrowsMeaningfulException()
    {
      var board = new GameBoardManager(BoardSizes.Standard);
      var diceRoller = new Dice();
      var cardPile = new DevelopmentCardPile();

      Should.Throw<ArgumentOutOfRangeException>(() => new GameSession(board, 5, diceRoller, cardPile))
        .Message.ShouldBe("Maximum Player count must be within range 2-4 inclusive. Was 5.");
    }

    [Test]
    public void RegisterClient_GameSessionIsEmpty_ClientIsRegistered()
    {
      var board = new GameBoardManager(BoardSizes.Standard);
      var diceRoller = new Dice();
      var cardPile = new DevelopmentCardPile();
      IGameSession gameManager = new GameSession(board, 2, diceRoller, cardPile);

      var clientAccount = new ClientAccount();

      var result = gameManager.RegisterPlayer(clientAccount);

      result.ShouldBeTrue();
    }

    [Test]
    public void RegisterClient_GameSessionNeedsLastPlayer_ClientIsRegistered()
    {
      var board = new GameBoardManager(BoardSizes.Standard);
      var diceRoller = new Dice();
      var cardPile = new DevelopmentCardPile();
      IGameSession gameManager = new GameSession(board, 2, diceRoller, cardPile);

      var clientAccount1 = new ClientAccount();
      var clientAccount2 = new ClientAccount();

      gameManager.RegisterPlayer(clientAccount1);
      var result = gameManager.RegisterPlayer(clientAccount2);

      result.ShouldBeTrue();
    }

    [Test]
    public void RegisterClient_GameSessionIsFull_ClientIsNotRegistered()
    {
      var board = new GameBoardManager(BoardSizes.Standard);
      var diceRoller = new Dice();
      var cardPile = new DevelopmentCardPile();
      IGameSession gameManager = new GameSession(board, 2, diceRoller, cardPile);

      var clientAccount1 = new ClientAccount();
      var clientAccount2 = new ClientAccount();
      var clientAccount3 = new ClientAccount();
      gameManager.RegisterPlayer(clientAccount1);
      gameManager.RegisterPlayer(clientAccount2);
      var result = gameManager.RegisterPlayer(clientAccount3);

      result.ShouldBeTrue();
    }

    [Test]
    public void GetFirstSetupPassOrder_ReturnsPlayerOrderBasedOnDiceRolls()
    {
      var diceRoller = Substitute.For<INumberGenerator>();
      diceRoller.RollTwoDice().Returns(4u, 8u, 6u, 10u);
      var gameManager = new GameSession(new GameBoardManager(BoardSizes.Standard), 4, diceRoller, new DevelopmentCardPile());


      gameManager.GetFirstSetupPassOrder().ShouldBe(new [] { 3u, 1u, 2u, 0u });
    }

    [Test]
    public void GetFirstSetupPassOrder_SameRollForTwoPlayersCausesReroll_ReturnsPlayerOrderBasedOnDiceRolls()
    {
      var diceRoller = Substitute.For<INumberGenerator>();
      diceRoller.RollTwoDice().Returns(10u, 8u, 6u, 10u, 12u);
      var gameManager = new GameSession(new GameBoardManager(BoardSizes.Standard), 4, diceRoller, new DevelopmentCardPile());


      gameManager.GetFirstSetupPassOrder().ShouldBe(new[] { 3u, 0u, 1u, 2u });
    }

    [Test]
    public void GetFirstSetupPassOrder_SameRollCausesTwoRoundsOfRerolls_ReturnsPlayerOrderBasedOnDiceRolls()
    {
      var diceRoller = Substitute.For<INumberGenerator>();
      diceRoller.RollTwoDice().Returns(10u, 10u, 10u, 10u, 7u, 6u, 7u, 8u);
      var gameManager = new GameSession(new GameBoardManager(BoardSizes.Standard), 4, diceRoller, new DevelopmentCardPile());


      gameManager.GetFirstSetupPassOrder().ShouldBe(new[] { 0u, 3u, 1u, 2u });
    }
    #endregion 
  }

  [TestFixture]
  public class GameController_UnitTests
  {
    #region Methods
    [Test]
    public void Test()
    {
      var mockGameManager = NSubstitute.Substitute.For<IGameSession>();
    }
    #endregion
  }
}
