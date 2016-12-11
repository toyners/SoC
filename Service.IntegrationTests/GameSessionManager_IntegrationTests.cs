
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameSessionManager_IntegrationTests
  {
    #region Methods
    [SetUp]
    public void SetupBeforeEachTest()
    {
      MockClient.SetupBeforeEachTest();
    }

    [Test]
    public void AddClient_AddPlayerToNonFullSession_PlayerAdded()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory());
      var mockClient = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient);
      Thread.Sleep(1000);

      this.WaitUntilGameSessionManagerStops(gameSessionManager);

      // Assert
      mockClient.GameJoined.ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
    {
      // Arrange
      Guid token = Guid.Empty;
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      this.WaitUntilGameSessionManagerStops(gameSessionManager);

      // Assert
      mockClient1.GameToken.ShouldNotBe(Guid.Empty);
      (mockClient1.GameToken == mockClient2.GameToken &&
       mockClient2.GameToken == mockClient3.GameToken &&
       mockClient3.GameToken == mockClient4.GameToken).ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersAreInitialized()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

      this.WaitUntilGameSessionManagerStops(gameSessionManager);

      // Assert
      mockClient1.GameInitialized.ShouldBeTrue();
      mockClient2.GameInitialized.ShouldBeTrue();
      mockClient3.GameInitialized.ShouldBeTrue();
      mockClient4.GameInitialized.ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 6u, 4u, 12u };
      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      MockClient[] clients = { mockClient1, mockClient2, mockClient3, mockClient4 };
      MockClient[] expectedOrder = { mockClient4, mockClient1, mockClient2, mockClient3 };

      ClientsReceivePlaceTownMessage(diceRolls, clients, expectedOrder);
    }

    [Test]
    public void Test()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient1);
      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient1);
      gameSessionManager.ConfirmGameInitialized(mockClient3.GameToken, mockClient3);
      gameSessionManager.ConfirmGameInitialized(mockClient4.GameToken, mockClient4);
      Thread.Sleep(1000);

      this.WaitUntilGameSessionManagerStops(gameSessionManager);

      mockClient1.PlaceTownMessageReceived.ShouldBeFalse();
      mockClient2.PlaceTownMessageReceived.ShouldBeFalse();
      mockClient3.PlaceTownMessageReceived.ShouldBeFalse();
      mockClient4.PlaceTownMessageReceived.ShouldBeFalse();
    }

    private void ClientsReceivePlaceTownMessage(List<UInt32> diceRolls, MockClient[] clients, MockClient[] expectedOrder)
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        Guid gameToken = Guid.Empty;
        var diceRollerFactory = this.CreateOneDiceRoller(diceRolls);
        gameSessionManager = this.CreateGameSessionManager(diceRollerFactory, 4);

        var mockClient1 = clients[0];
        var mockClient2 = clients[1];
        var mockClient3 = clients[2];
        var mockClient4 = clients[3];

        // Act
        gameSessionManager.AddClient(mockClient1);
        gameSessionManager.AddClient(mockClient2);
        gameSessionManager.AddClient(mockClient3);
        gameSessionManager.AddClient(mockClient4);

        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient1);
        gameSessionManager.ConfirmGameInitialized(mockClient2.GameToken, mockClient2);
        gameSessionManager.ConfirmGameInitialized(mockClient3.GameToken, mockClient3);
        gameSessionManager.ConfirmGameInitialized(mockClient4.GameToken, mockClient4);

        this.WaitUntilClientReceivesPlaceTownMessage(expectedOrder[0]);
        gameSessionManager.ConfirmTownPlaced(expectedOrder[0].GameToken, expectedOrder[0]);
        this.WaitUntilClientReceivesPlaceTownMessage(expectedOrder[1]);
        gameSessionManager.ConfirmTownPlaced(expectedOrder[1].GameToken, expectedOrder[1]);
        this.WaitUntilClientReceivesPlaceTownMessage(expectedOrder[2]);
        gameSessionManager.ConfirmTownPlaced(expectedOrder[2].GameToken, expectedOrder[2]);
        this.WaitUntilClientReceivesPlaceTownMessage(expectedOrder[3]);
        gameSessionManager.ConfirmTownPlaced(expectedOrder[3].GameToken, expectedOrder[3]);

        this.WaitUntilGameSessionManagerStops(gameSessionManager);

        // Assert
        expectedOrder[0].TownPlacedRank.ShouldBe(1u);
        expectedOrder[1].TownPlacedRank.ShouldBe(2u);
        expectedOrder[2].TownPlacedRank.ShouldBe(3u);
        expectedOrder[3].TownPlacedRank.ShouldBe(4u);
      }
      finally
      {
        this.WaitUntilGameSessionManagerStops(gameSessionManager);
      }
    }

    private IDiceRollerFactory CreateOneDiceRoller(List<UInt32> diceRolls)
    {
      var index = 0;
      var diceRoller = Substitute.For<IDiceRoller>();
      diceRoller.RollTwoDice().Returns(x => { return diceRolls[index++]; });

      var diceRollerFactory = Substitute.For<IDiceRollerFactory>();
      diceRollerFactory.Create().Returns(diceRoller);

      return diceRollerFactory;
    }

    private GameSessionManager CreateGameSessionManager(IDiceRollerFactory diceRollerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(diceRollerFactory, maximumPlayerCount);
      gameSessionManager.Start();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      // Wait until the game session manager is started before continuing. Set a limit of 5 seconds for this to happen.
      while (gameSessionManager.State != GameSessionManager.States.Running && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      // Still not started.
      if (gameSessionManager.State != GameSessionManager.States.Running)
      {
        throw new Exception("GameSessionManager has not started");
      }
      
      return gameSessionManager;
    }

    private void WaitUntilClientReceivesPlaceTownMessage(MockClient mockClient)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      while (!mockClient.PlaceTownMessageReceived && stopWatch.ElapsedMilliseconds <= 2000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      if (!mockClient.PlaceTownMessageReceived)
      {
        throw new TimeoutException("Timed out waiting for client to receive place town message.");
      }
    }

    private void WaitUntilClientsReceiveGameData(params MockClient[] mockClients)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      var waitingForGameData = new List<MockClient>(mockClients);

      while (waitingForGameData.Count > 0 && stopWatch.ElapsedMilliseconds <= 1000)
      {
        for (var index = 0; index < waitingForGameData.Count; index++)
        {
          if (waitingForGameData[index].GameInitialized)
          {
            waitingForGameData.RemoveAt(index);
            index--;
          }
        }

        Thread.Sleep(50);
      }

      stopWatch.Stop();

      if (waitingForGameData.Count > 0)
      {
        throw new TimeoutException("Timed out waiting for clients to receive game data.");
      }
    }

    private void WaitUntilGameSessionManagerStops(GameSessionManager gameSessionManager)
    {
      gameSessionManager.Stop();
      while (gameSessionManager.State != GameSessionManager.States.Stopped)
      {
        Thread.Sleep(50);
      }
    }
    #endregion 
  }
}
