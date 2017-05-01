
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
      throw new NotImplementedException();
    }

    [Test]
    public void RegisterPlayer_GameManagerHasOneSlotLeft_PlayerIsRegistered()
    {
      throw new NotImplementedException();
    }

    [Test]
    public void RegisterPlayer_GameManagerIsFull_PlayerIsNotRegistered()
    {
      throw new NotImplementedException();
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
