
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
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
      var gameSessionManager = this.CreateGameSessionManager(new GameManagerFactory(), 1);
      var mockClient = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient);
      Thread.Sleep(1000);

      this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

      // Assert
      mockClient.GameJoined.ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
    {
      // Arrange
      Guid token = Guid.Empty;
      var gameSessionManager = this.CreateGameSessionManager(new GameManagerFactory(), 4);

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

      this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

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
      var gameSessionManager = this.CreateGameSessionManager(new GameManagerFactory(), 4);

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

      this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

      // Assert
      mockClient1.GameInitialized.ShouldBeTrue();
      mockClient2.GameInitialized.ShouldBeTrue();
      mockClient3.GameInitialized.ShouldBeTrue();
      mockClient4.GameInitialized.ShouldBeTrue();
    }

    [Test]
    [TestCase(new UInt32[] { 0u, 1u, 2u, 3u })]
    [TestCase(new UInt32[] { 3u, 0u, 1u, 2u })]
    [TestCase(new UInt32[] { 2u, 3u, 0u, 1u })]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 0u })]
    public void ClientsReceivePlaceTownMessageInCorrectOrder(UInt32[] firstSetupPassOrder)
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var board = new Board(BoardSizes.Standard);
        var mockGameManager = Substitute.For<IGameManager>();
        mockGameManager.GetFirstSetupPassOrder().Returns(firstSetupPassOrder);
        mockGameManager.Board.Returns(board);

        var mockGameManagerFactory = Substitute.For<IGameManagerFactory>();
        mockGameManagerFactory.Create().Returns(mockGameManager);

        gameSessionManager = this.CreateGameSessionManager(mockGameManagerFactory, 4);

        var clients = new[] { new MockClient(), new MockClient(), new MockClient(), new MockClient() };

        var mockClient1 = clients[0];
        var mockClient2 = clients[1];
        var mockClient3 = clients[2];
        var mockClient4 = clients[3];

        var firstMockClient = clients[firstSetupPassOrder[0]];
        var secondMockClient = clients[firstSetupPassOrder[1]];
        var thirdMockClient = clients[firstSetupPassOrder[2]];
        var fourthMockClient = clients[firstSetupPassOrder[3]];

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

        firstMockClient.WaitUntilClientReceivesPlaceTownMessage();
        gameSessionManager.ConfirmTownPlacement(firstMockClient.GameToken, firstMockClient, 0u);

        secondMockClient.WaitUntilClientReceivesPlaceTownMessage();
        gameSessionManager.ConfirmTownPlacement(secondMockClient.GameToken, secondMockClient, 10u);

        thirdMockClient.WaitUntilClientReceivesPlaceTownMessage();
        gameSessionManager.ConfirmTownPlacement(thirdMockClient.GameToken, thirdMockClient, 20u);

        fourthMockClient.WaitUntilClientReceivesPlaceTownMessage();
        gameSessionManager.ConfirmTownPlacement(fourthMockClient.GameToken, fourthMockClient, 30u);
      }
      finally
      {
        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);
      }
    }

    /// <summary>
    /// If a client sends multiple game initialization confirmation messages then the
    /// subsequent messages should be ignored. In this scenario the second client 
    /// sends nothing so the same number of messages are being sent to the server.
    /// </summary>
    [Test]
    public void SubsequentGameInitializationConfirminationMessagesAreIgnored()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new GameManagerFactory(), 4);

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

      this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

      mockClient1.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient2.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient3.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient4.ChooseTownLocationMessageReceived.ShouldBeFalse();
    }

    /// <summary>
    /// If a client sends the wrong message when the server is waiting for 
    /// game initialization confirmation messages then the message should be ignored. 
    /// In this scenario the first client sends the wrong message.
    /// </summary>
    [Test]
    public void WhenWaitingForGameInitializationConfirminationMessagesWrongMessagesAreIgnored()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new GameManagerFactory(), 4);

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

      gameSessionManager.ConfirmTownPlacement(mockClient1.GameToken, mockClient1, 0u);
      gameSessionManager.ConfirmGameInitialized(mockClient2.GameToken, mockClient2);
      gameSessionManager.ConfirmGameInitialized(mockClient3.GameToken, mockClient3);
      gameSessionManager.ConfirmGameInitialized(mockClient4.GameToken, mockClient4);
      Thread.Sleep(1000);

      this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

      mockClient1.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient2.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient3.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient4.ChooseTownLocationMessageReceived.ShouldBeFalse();
    }

    /// <summary>
    /// Game manager receives the correct location from the clients when placing 
    /// the town.
    /// </summary>
    [Test]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 4u })]
    [TestCase(new UInt32[] { 1u, 5u, 9u, 30u })]
    [TestCase(new UInt32[] { 3u, 12u, 20u, 35u })]
    public void GameManagerReceivesCorrectMessagesWhenPlacingFirstTown(UInt32[] townLocations)
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var board = new Board(BoardSizes.Standard);
        var mockGameManager = Substitute.For<IGameManager>();
        mockGameManager.GetFirstSetupPassOrder().Returns(new UInt32[] { 0u, 1u, 2u, 3u });
        mockGameManager.Board.Returns(board);

        var mockGameManagerFactory = Substitute.For<IGameManagerFactory>();
        mockGameManagerFactory.Create().Returns(mockGameManager);

        gameSessionManager = this.CreateGameSessionManager(mockGameManagerFactory, 4);

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
        gameSessionManager.ConfirmGameInitialized(mockClient2.GameToken, mockClient2);
        gameSessionManager.ConfirmGameInitialized(mockClient3.GameToken, mockClient3);
        gameSessionManager.ConfirmGameInitialized(mockClient4.GameToken, mockClient4);

        mockClient1.WaitUntilClientReceivesPlaceTownMessage();
        gameSessionManager.ConfirmTownPlacement(mockClient1.GameToken, mockClient1, townLocations[0]);

        mockClient2.WaitUntilClientReceivesPlaceTownMessage();
        mockGameManager.Received().PlaceTown(townLocations[0]);
        gameSessionManager.ConfirmTownPlacement(mockClient2.GameToken, mockClient2, townLocations[1]);

        mockClient3.WaitUntilClientReceivesPlaceTownMessage();
        mockGameManager.Received().PlaceTown(townLocations[1]);
        gameSessionManager.ConfirmTownPlacement(mockClient3.GameToken, mockClient3, townLocations[2]);

        mockClient4.WaitUntilClientReceivesPlaceTownMessage();
        mockGameManager.Received().PlaceTown(townLocations[2]);
        gameSessionManager.ConfirmTownPlacement(mockClient4.GameToken, mockClient4, townLocations[3]);

        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);
        mockGameManager.Received().PlaceTown(townLocations[3]);
      }
      finally
      {
        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);
      }
    }

    [Test]
    [TestCase(new UInt32[] { 0u, 1u, 2u, 3u }, new UInt32[] { 1u, 5u, 13u, 27u })]
    [TestCase(new UInt32[] { 3u, 0u, 1u, 2u }, new UInt32[] { 7u, 10u, 1u, 19u })]
    [TestCase(new UInt32[] { 2u, 3u, 0u, 1u }, new UInt32[] { 13u, 26u, 5u, 9u })]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 0u }, new UInt32[] { 26u, 11u, 4u, 15u })]
    public void CompleteFirstRoundTownPlacement(UInt32[] setupOrder, UInt32[] locationIndexes)
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var board = new Board(BoardSizes.Standard);
        var mockGameManager = Substitute.For<IGameManager>();
        mockGameManager.GetFirstSetupPassOrder().Returns(setupOrder);
        mockGameManager.Board.Returns(board);

        var mockGameManagerFactory = Substitute.For<IGameManagerFactory>();
        mockGameManagerFactory.Create().Returns(mockGameManager);

        gameSessionManager = this.CreateGameSessionManager(mockGameManagerFactory, 4);

        var mockClients = new[] 
        {
          new MockClient(gameSessionManager),
          new MockClient(gameSessionManager),
          new MockClient(gameSessionManager),
          new MockClient(gameSessionManager)
        };

        var mockClient1 = mockClients[0];
        var mockClient2 = mockClients[1];
        var mockClient3 = mockClients[2];
        var mockClient4 = mockClients[3];


        var firstMockClient = mockClients[setupOrder[0]];
        var secondMockClient = mockClients[setupOrder[1]];
        var thirdMockClient = mockClients[setupOrder[2]];
        var fourthMockClient = mockClients[setupOrder[3]];

        // Act
        this.AddClientsToSessionManager(gameSessionManager, mockClient1, mockClient2, mockClient3, mockClient4);
        
        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        this.ConfirmGameInitializedForClients(gameSessionManager, mockClient1, mockClient2, mockClient3, mockClient4);

        var locationIndex = 0;
        var location = locationIndexes[locationIndex];
        var expectedSelectedTownLocations = new List<UInt32>();

        firstMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        firstMockClient.PlaceTown(location);

        expectedSelectedTownLocations.Add(location);
        location = locationIndexes[++locationIndex];
        secondMockClient.WaitUntilClientReceivesPlaceTownMessage();
        secondMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        secondMockClient.PlaceTown(location);

        expectedSelectedTownLocations.Add(location);
        var lastLocation = location;
        location = locationIndexes[++locationIndex];
        thirdMockClient.WaitUntilClientReceivesPlaceTownMessage();
        thirdMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        firstMockClient.NewTownLocation.ShouldBe(lastLocation);
        thirdMockClient.PlaceTown(location);

        expectedSelectedTownLocations.Add(location);
        lastLocation = location;
        location = locationIndexes[++locationIndex];
        fourthMockClient.WaitUntilClientReceivesPlaceTownMessage();
        fourthMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        firstMockClient.NewTownLocation.ShouldBe(lastLocation);
        secondMockClient.NewTownLocation.ShouldBe(lastLocation);
        fourthMockClient.PlaceTown(location);

        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

        lastLocation = location;
        firstMockClient.NewTownLocation.ShouldBe(lastLocation);
        secondMockClient.NewTownLocation.ShouldBe(lastLocation);
        thirdMockClient.NewTownLocation.ShouldBe(lastLocation);
      }
      finally
      {
        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);
      }
    }

    public void CompleteBothRoundsOfTownPlacement(UInt32[] setupOrder, UInt32[] locationIndexes)
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var board = new Board(BoardSizes.Standard);
        var mockGameManager = Substitute.For<IGameManager>();
        mockGameManager.GetFirstSetupPassOrder().Returns(setupOrder);
        mockGameManager.Board.Returns(board);

        var mockGameManagerFactory = Substitute.For<IGameManagerFactory>();
        mockGameManagerFactory.Create().Returns(mockGameManager);

        gameSessionManager = this.CreateGameSessionManager(mockGameManagerFactory, 4);

        var mockClients = new[]
        {
          new MockClient(gameSessionManager),
          new MockClient(gameSessionManager),
          new MockClient(gameSessionManager),
          new MockClient(gameSessionManager)
        };

        var mockClient1 = mockClients[0];
        var mockClient2 = mockClients[1];
        var mockClient3 = mockClients[2];
        var mockClient4 = mockClients[3];


        var firstMockClient = mockClients[setupOrder[0]];
        var secondMockClient = mockClients[setupOrder[1]];
        var thirdMockClient = mockClients[setupOrder[2]];
        var fourthMockClient = mockClients[setupOrder[3]];

        // Act
        this.AddClientsToSessionManager(gameSessionManager, mockClient1, mockClient2, mockClient3, mockClient4);

        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        this.ConfirmGameInitializedForClients(gameSessionManager, mockClient1, mockClient2, mockClient3, mockClient4);

        var locationIndex = 0;
        var location = locationIndexes[locationIndex];
        var expectedSelectedTownLocations = new List<UInt32>();

        firstMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        firstMockClient.PlaceTown(location);

        expectedSelectedTownLocations.Add(location);
        location = locationIndexes[++locationIndex];
        secondMockClient.WaitUntilClientReceivesPlaceTownMessage();
        secondMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        secondMockClient.PlaceTown(location);

        expectedSelectedTownLocations.Add(location);
        var lastLocation = location;
        location = locationIndexes[++locationIndex];
        thirdMockClient.WaitUntilClientReceivesPlaceTownMessage();
        thirdMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        firstMockClient.NewTownLocation.ShouldBe(lastLocation);
        thirdMockClient.PlaceTown(location);

        expectedSelectedTownLocations.Add(location);
        lastLocation = location;
        location = locationIndexes[++locationIndex];
        fourthMockClient.WaitUntilClientReceivesPlaceTownMessage();
        fourthMockClient.SelectedTownLocations.ShouldBe(expectedSelectedTownLocations);
        firstMockClient.NewTownLocation.ShouldBe(lastLocation);
        secondMockClient.NewTownLocation.ShouldBe(lastLocation);
        fourthMockClient.PlaceTown(location);

        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);

        lastLocation = location;
        firstMockClient.NewTownLocation.ShouldBe(lastLocation);
        secondMockClient.NewTownLocation.ShouldBe(lastLocation);
        thirdMockClient.NewTownLocation.ShouldBe(lastLocation);
      }
      finally
      {
        this.WaitUntilGameSessionManagerHasStopped(gameSessionManager);
      }
    }

    private void AddClientsToSessionManager(GameSessionManager gameSessionManager, params MockClient[] mockClients)
    {
      foreach (var mockClient in mockClients)
      {
        gameSessionManager.AddClient(mockClient);
      }
    }

    private void ConfirmGameInitializedForClients(GameSessionManager gameSessionManager, params MockClient[] mockClients)
    {
      foreach (var mockClient in mockClients)
      {
        gameSessionManager.ConfirmGameInitialized(mockClient.GameToken, mockClient);
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

    private GameSessionManager CreateGameSessionManager(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(gameManagerFactory, maximumPlayerCount);
      this.WaitUntilGameSessionManagerHasStarted(gameSessionManager);
      return gameSessionManager;
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

    private void WaitUntilGameSessionManagerHasStarted(GameSessionManager gameSessionManager)
    {
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
        throw new Exception("GameSessionManager has not started.");
      }
    }

    private void WaitUntilGameSessionManagerHasStopped(GameSessionManager gameSessionManager)
    {
      if (gameSessionManager.State == GameSessionManager.States.Stopped)
      {
        return;
      }

      gameSessionManager.Stop();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      // Wait until the game session manager is started before continuing. Set a limit of 5 seconds for this to happen.
      while (gameSessionManager.State != GameSessionManager.States.Stopped && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      // Still not stopped.
      if (gameSessionManager.State != GameSessionManager.States.Stopped)
      {
        throw new Exception("GameSessionManager has not stopped.");
      }
    }
    #endregion 
  }
}
