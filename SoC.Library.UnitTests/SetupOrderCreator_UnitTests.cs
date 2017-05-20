
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class SetupOrderCreator_UnitTests
  {
    #region Methods
    [Test]
    [TestCase(12u, 10u, 8u, 6u, 0, 1, 2, 3)]
    [TestCase(10u, 8u, 6u, 12u, 3, 0, 1, 2)]
    [TestCase(8u, 6u, 12u, 10u, 2, 3, 0, 1)]
    [TestCase(6u, 12u, 10u, 8u, 1, 2, 3, 0)]
    public void Create_DifferentRolls_ReturnsPlayersInDescendingOrder(UInt32 firstRoll, UInt32 secondRoll, UInt32 thirdRoll, UInt32 fourthRoll, Int32 first, Int32 second, Int32 third, Int32 fourth)
    {
      var player1 = new PlayerBase();
      var player2 = new PlayerBase();
      var player3 = new PlayerBase();
      var player4 = new PlayerBase();

      var players = new PlayerBase[] { player1, player2, player3, player4 };
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(firstRoll, secondRoll, thirdRoll, fourthRoll);
      var setupOrder = SetupOrderCreator.Create(players, mockDice);

      setupOrder.ShouldBe(new PlayerBase[] { players[first], players[second], players[third], players[fourth]});
    }

    [Test]
    [TestCase(12u, 12u, 10u, 8u, 0, 1, 2)]
    [TestCase(12u, 10u, 10u, 8u, 0, 1, 2)]
    [TestCase(12u, 10u, 12u, 8u, 0, 1, 2)]
    [TestCase(12u, 6u, 8u, 8u, 0, 2, 1)]
    public void Create_DuplicateRollsAreIgnored_ReturnsPlayersInDescendingOrder(UInt32 firstRoll, UInt32 secondRoll, UInt32 thirdRoll, UInt32 fourthRoll, Int32 first, Int32 second, Int32 third)
    {
      var players = new PlayerBase[] { new PlayerBase(), new PlayerBase(), new PlayerBase() };
      var mockDice = NSubstitute.Substitute.For<IDice>(); 
      mockDice.RollTwoDice().Returns(firstRoll, secondRoll, thirdRoll, fourthRoll);
      var setupOrder = SetupOrderCreator.Create(players, mockDice);

      setupOrder.ShouldBe(new PlayerBase[] { players[first], players[second], players[third] });
    }
    #endregion 
  }
}
