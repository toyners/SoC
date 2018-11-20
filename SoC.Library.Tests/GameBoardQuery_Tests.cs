
namespace Jabberwocky.SoC.Library.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.UnitTests.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("All")]
    [Category("GameBoardQuery")]
    public class GameBoardQuery_Tests
    {
        [Test]
        public void GetLocationsWithBestYield_FirstLocationFromEmptyBoard_ReturnsExpectedLocation()
        {
            var gameBoard = new GameBoard(BoardSizes.Standard);
            var queryEngine = new GameBoardQuery(gameBoard);

            var results = queryEngine.GetLocationsWithBestYield(1);

            results.ShouldContainExact(new[] { 12u });
        }

        [Test]
        public void GetLocationsWithBestYield_FirstFiveLocationsFromEmptyBoard_ReturnsExpectedLocations()
        {
            var gameBoard = new GameBoard(BoardSizes.Standard);
            var queryEngine = new GameBoardQuery(gameBoard);

            var results = queryEngine.GetLocationsWithBestYield(5);

            results.ShouldContainExact(new[] { 12u, 31u, 35u, 41u, 43u });
        }

        [Test]
        [TestCase(12u, 4u, 31u, 35u, 41u, 43u, 18u)]
        [TestCase(31u, 20u, 12u, 35u, 41u, 43u, 11u)]
        [TestCase(35u, 24u, 12u, 31u, 41u, 43u, 11u)]
        [TestCase(41u, 49u, 12u, 31u, 35u, 43u, 11u)]
        [TestCase(43u, 44u, 12u, 31u, 35u, 41u, 11u)]
        [TestCase(11u, 10u, 31u, 35u, 41u, 43u, 18u)]
        public void GetLocationsWithBestYield_FiveLocationsWhenLocationIsTaken_ReturnsExpectedLocations(uint settlementLocation, uint roadEndLocation, uint firstLocation, uint secondLocation, uint thirdLocation, uint fourthLocation, uint fifthLocation)
        {
            var gameBoard = new GameBoard(BoardSizes.Standard);
            gameBoard.PlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation);
            var queryEngine = new GameBoardQuery(gameBoard);

            var results = queryEngine.GetLocationsWithBestYield(5);

            results.ShouldContainExact(new[] { firstLocation, secondLocation, thirdLocation, fourthLocation, fifthLocation });
        }

        [Test]
        public void GetValidConnectionsForPlayerInfrastructure_NoInfrastructure_ReturnsExpectedException()
        {
            var gameBoard = new GameBoard(BoardSizes.Standard);
            var queryEngine = new GameBoardQuery(gameBoard);
            var playerId = Guid.NewGuid();
            Action action = () => { queryEngine.GetValidConnectionsForPlayerInfrastructure(playerId); };
            action.ShouldThrow<Exception>().Message.ShouldBe($"Player {playerId} not recognised.");
        }

        [Test]
        public void GetValidConnectionsForPlayerInfrastructure_StartingInfrastructurePlaced_ExpectedConnections()
        {
            var gameBoard = new GameBoard(BoardSizes.Standard);
            var queryEngine = new GameBoardQuery(gameBoard);
            var playerId = Guid.NewGuid();
            gameBoard.PlaceStartingInfrastructure(playerId, 0, 8);
            gameBoard.PlaceStartingInfrastructure(playerId, 19, 18);

            var results = queryEngine.GetValidConnectionsForPlayerInfrastructure(playerId);

            var expected = new HashSet<Connection>();
            expected.Add(new Connection(0, 1));
            expected.Add(new Connection(8, 7));
            expected.Add(new Connection(8, 9));
            results.ShouldBe(expected);
        }
    }
}
