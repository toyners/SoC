
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Interfaces;
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
      MockClient3.SetupBeforeEachTest();
    }

    /// <summary>
    /// All clients receive player cards for all clients in game session.
    /// </summary>
    //[Test]
    public void AllClientsReceivePlayerCardsForAllClientsInGame()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var username1 = "User1";
        var expectedPlayerData1 = new PlayerData(username1);

        var username2 = "User2";
        var expectedPlayerData2 = new PlayerData(username2);

        var username3 = "User3";
        var expectedPlayerData3 = new PlayerData(username3);

        var username4 = "User4";
        var expectedPlayerData4 = new PlayerData(username4);

        var mockPlayerCardRepository = Substitute.For<IPlayerCardRepository>();
        mockPlayerCardRepository.GetPlayerData(username1).Returns(expectedPlayerData1);
        mockPlayerCardRepository.GetPlayerData(username2).Returns(expectedPlayerData2);
        mockPlayerCardRepository.GetPlayerData(username3).Returns(expectedPlayerData3);
        mockPlayerCardRepository.GetPlayerData(username4).Returns(expectedPlayerData4);

        var mockClient1 = new MockClient3 { Username = username1 };
        var mockClient2 = new MockClient3 { Username = username2 };
        var mockClient3 = new MockClient3 { Username = username3 };
        var mockClient4 = new MockClient3 { Username = username4 };

        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4, mockPlayerCardRepository);

        // Act
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);
        Thread.Sleep(1000);

        // Assert
        mockClient1.ReceivedPlayerData.Count.ShouldBe(3);
        mockClient1.ReceivedPlayerData.ShouldBe(new[] { expectedPlayerData2, expectedPlayerData3, expectedPlayerData4 }, true);

        mockClient2.ReceivedPlayerData.Count.ShouldBe(3);
        mockClient2.ReceivedPlayerData.ShouldBe(new[] { expectedPlayerData1, expectedPlayerData3, expectedPlayerData4 }, true);

        mockClient3.ReceivedPlayerData.Count.ShouldBe(3);
        mockClient3.ReceivedPlayerData.ShouldBe(new[] { expectedPlayerData1, expectedPlayerData2, expectedPlayerData4 }, true);

        mockClient4.ReceivedPlayerData.Count.ShouldBe(3);
        mockClient4.ReceivedPlayerData.ShouldBe(new[] { expectedPlayerData1, expectedPlayerData2, expectedPlayerData3 }, true);
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    //[Test]
    public void SameClientCantBeAddedToSameGameSession()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);
      }
      finally
      {
        if (gameSessionManager != null)
        {
          gameSessionManager.WaitUntilGameSessionManagerHasStopped();
        }
      }
    }

    //[Test]
    public void AllClientsReceiveSameGameTokenWhenJoinedToSameGame()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);

        var mockClient1 = new MockClient3();
        var mockClient2 = new MockClient3();
        var mockClient3 = new MockClient3();
        var mockClient4 = new MockClient3();

        // Act
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);
        Thread.Sleep(1000);

        gameSessionManager.WaitUntilGameSessionManagerHasStopped();

        // Assert
        mockClient1.GameToken.ShouldNotBe(Guid.Empty);
        (mockClient1.GameToken == mockClient2.GameToken &&
         mockClient2.GameToken == mockClient3.GameToken &&
         mockClient3.GameToken == mockClient4.GameToken).ShouldBeTrue();
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    //[Test]
    public void AllClientsReceiveBoardDataWhenGameIsFull()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);

        var mockClient1 = new MockClient3();
        var mockClient2 = new MockClient3();
        var mockClient3 = new MockClient3();
        var mockClient4 = new MockClient3();

        // Act
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);

        // Assert
        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /*[Test]
    [TestCase(new UInt32[] { 0u, 1u, 2u, 3u })]
    [TestCase(new UInt32[] { 3u, 0u, 1u, 2u })]
    [TestCase(new UInt32[] { 2u, 3u, 0u, 1u })]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 0u })]*/
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

        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

        var clients = new[] { new MockClient3(gameSessionManager), new MockClient3(gameSessionManager), new MockClient3(gameSessionManager), new MockClient3(gameSessionManager) };

        var mockClient1 = clients[0];
        var mockClient2 = clients[1];
        var mockClient3 = clients[2];
        var mockClient4 = clients[3];

        var firstMockClient = clients[firstSetupPassOrder[0]];
        var secondMockClient = clients[firstSetupPassOrder[1]];
        var thirdMockClient = clients[firstSetupPassOrder[2]];
        var fourthMockClient = clients[firstSetupPassOrder[3]];

        // Act
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);

        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        this.ConfirmGameInitializedForClients(mockClient1, mockClient2, mockClient3, mockClient4);
        
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
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /// <summary>
    /// If a client sends multiple game initialization confirmation messages then the
    /// subsequent messages should be ignored. In this scenario the second client 
    /// sends nothing so the same number of messages are being sent to the server.
    /// </summary>
    //[Test]
    public void SubsequentGameInitializationConfirminationMessagesAreIgnored()
    {
      // Arrange
      var gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);

      var mockClient1 = new MockClient3();
      var mockClient2 = new MockClient3();
      var mockClient3 = new MockClient3();
      var mockClient4 = new MockClient3();

      // Act
      gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);

      this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient1);
      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient1);
      gameSessionManager.ConfirmGameInitialized(mockClient3.GameToken, mockClient3);
      gameSessionManager.ConfirmGameInitialized(mockClient4.GameToken, mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.WaitUntilGameSessionManagerHasStopped();

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
    //[Test]
    public void WhenWaitingForGameInitializationConfirminationMessagesWrongMessagesAreIgnored()
    {
      // Arrange
      var gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);

      var mockClient1 = new MockClient3();
      var mockClient2 = new MockClient3();
      var mockClient3 = new MockClient3();
      var mockClient4 = new MockClient3();

      // Act
      gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);

      this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

      gameSessionManager.ConfirmTownPlacement(mockClient1.GameToken, mockClient1, 0u);
      gameSessionManager.ConfirmGameInitialized(mockClient2.GameToken, mockClient2);
      gameSessionManager.ConfirmGameInitialized(mockClient3.GameToken, mockClient3);
      gameSessionManager.ConfirmGameInitialized(mockClient4.GameToken, mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.WaitUntilGameSessionManagerHasStopped();

      mockClient1.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient2.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient3.ChooseTownLocationMessageReceived.ShouldBeFalse();
      mockClient4.ChooseTownLocationMessageReceived.ShouldBeFalse();
    }

    /// <summary>
    /// Game manager receives the correct location from the clients when placing 
    /// the town.
    /// </summary>
    /*[Test]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 4u })]
    [TestCase(new UInt32[] { 1u, 5u, 9u, 30u })]
    [TestCase(new UInt32[] { 3u, 12u, 20u, 35u })]*/
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

        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

        var mockClient1 = new MockClient3(gameSessionManager);
        var mockClient2 = new MockClient3(gameSessionManager);
        var mockClient3 = new MockClient3(gameSessionManager);
        var mockClient4 = new MockClient3(gameSessionManager);

        // Act
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);

        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        this.ConfirmGameInitializedForClients(mockClient1, mockClient2, mockClient3, mockClient4);

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

        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
        mockGameManager.Received().PlaceTown(townLocations[3]);
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /*[Test]
    [TestCase(new UInt32[] { 0u, 1u, 2u, 3u }, new UInt32[] { 1u, 5u, 13u, 27u })]
    [TestCase(new UInt32[] { 3u, 0u, 1u, 2u }, new UInt32[] { 7u, 10u, 1u, 19u })]
    [TestCase(new UInt32[] { 2u, 3u, 0u, 1u }, new UInt32[] { 13u, 26u, 5u, 9u })]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 0u }, new UInt32[] { 26u, 11u, 4u, 15u })]*/
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

        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

        var mockClients = new[] 
        {
          new MockClient3(gameSessionManager),
          new MockClient3(gameSessionManager),
          new MockClient3(gameSessionManager),
          new MockClient3(gameSessionManager)
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
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);
        
        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        this.ConfirmGameInitializedForClients(mockClient1, mockClient2, mockClient3, mockClient4);

        var locationIndex = 0;
        var location = locationIndexes[locationIndex];

        firstMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.PlaceTown(location);

        secondMockClient.WaitUntilClientReceivesPlaceTownMessage();
        secondMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        secondMockClient.PlaceTown(location);

        thirdMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        thirdMockClient.PlaceTown(location);

        fourthMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        secondMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        fourthMockClient.PlaceTown(location);

        gameSessionManager.WaitUntilGameSessionManagerHasStopped();

        firstMockClient.NewTownLocation.ShouldBe(location);
        secondMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /*[Test]
    [TestCase(new UInt32[] { 0u, 1u, 2u, 3u }, new UInt32[] { 1u, 5u, 13u, 27u, 3u, 7u, 17u, 30u })]
    [TestCase(new UInt32[] { 3u, 0u, 1u, 2u }, new UInt32[] { 15u, 3u, 12u, 36u, 11u, 6u, 22u, 30u })]
    [TestCase(new UInt32[] { 2u, 3u, 0u, 1u }, new UInt32[] { 10u, 2u, 14u, 23u, 33u, 9u, 21u, 37u })]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 0u }, new UInt32[] { 34u, 26u, 16u, 20u, 34u, 8u, 0u, 40u })]*/
    public void CompleteBothRoundsOfTownPlacement(UInt32[] setupOrder, UInt32[] locationIndexes)
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var board = new Board(BoardSizes.Standard);
        var mockGameManager = Substitute.For<IGameManager>();
        mockGameManager.GetFirstSetupPassOrder().Returns(setupOrder);
        var secondSetupOrder = new List<UInt32>(setupOrder);
        secondSetupOrder.Reverse();
         
        mockGameManager.GetSecondSetupPassOrder().Returns(secondSetupOrder.ToArray());
        mockGameManager.Board.Returns(board);

        var mockGameManagerFactory = Substitute.For<IGameManagerFactory>();
        mockGameManagerFactory.Create().Returns(mockGameManager);

        gameSessionManager = GameSessionManagerExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

        var mockClients = new[]
        {
          new MockClient3(gameSessionManager),
          new MockClient3(gameSessionManager),
          new MockClient3(gameSessionManager),
          new MockClient3(gameSessionManager)
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
        gameSessionManager.AddMockClients(mockClient1, mockClient2, mockClient3, mockClient4);

        this.WaitUntilClientsReceiveGameData(mockClient1, mockClient2, mockClient3, mockClient4);

        this.ConfirmGameInitializedForClients(mockClient1, mockClient2, mockClient3, mockClient4);

        var locationIndex = 0;
        var location = locationIndexes[locationIndex];

        firstMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.PlaceTown(location);

        secondMockClient.WaitUntilClientReceivesPlaceTownMessage();
        secondMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        secondMockClient.PlaceTown(location);

        thirdMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        thirdMockClient.PlaceTown(location);

        fourthMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        secondMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        fourthMockClient.PlaceTown(location);

        // Start of round two
        fourthMockClient.WaitUntilClientReceivesPlaceTownMessage(); 
        firstMockClient.NewTownLocation.ShouldBe(location);
        secondMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        fourthMockClient.PlaceTown(location);

        thirdMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        secondMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        thirdMockClient.PlaceTown(location);

        secondMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        secondMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        secondMockClient.PlaceTown(location);

        firstMockClient.WaitUntilClientReceivesPlaceTownMessage();
        firstMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);

        location = locationIndexes[++locationIndex];
        firstMockClient.PlaceTown(location);

        gameSessionManager.WaitUntilGameSessionManagerHasStopped();

        secondMockClient.NewTownLocation.ShouldBe(location);
        thirdMockClient.NewTownLocation.ShouldBe(location);
        fourthMockClient.NewTownLocation.ShouldBe(location);
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    private void ConfirmGameInitializedForClients(params MockClient3[] mockClients)
    {
      foreach (var mockClient in mockClients)
      {
        mockClient.ConfirmGameInitialized();
      }
    }

    private void WaitUntilClientsReceiveGameData(params MockClient3[] mockClients)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      var waitingForGameData = new List<MockClient3>(mockClients);

      while (waitingForGameData.Count > 0
#if !DEBUG
        && stopWatch.ElapsedMilliseconds <= 1000
#endif
        )
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
    #endregion
  }
}
