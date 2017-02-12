
namespace Service.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Service;
  using Jabberwocky.Toolkit.IO;
  using Messages;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameSessionManager_UnitTests
  {
    private const String TestPlayer1UserName = "Test Player 1";
    private const String TestPlayer2UserName = "Test Player 2";
    private const String TestPlayer3UserName = "Test Player 3";
    private const String TestPlayer4UserName = "Test Player 4";

    #region Methods
    [SetUp]
    public void SetupBeforeEachTest()
    {
      TestClient.SetupBeforeEachTest();
      DirectoryOperations.EnsureDirectoryIsEmpty(GameSessionManagerTestExtensions.GameSessionManagerUnitTestLoggingPath);
    }

    [Test]
    public void PlayerReceivesConfirmationOnceJoinedToGameSession()
    {
      // Arrange
      GameSessionManager gameSessionManager = null;
      try
      {
        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);

        // Act
        testPlayer1.JoinGame();
        Thread.Sleep(1000);

        // Assert
        var receivedMessage = testPlayer1.GetLastMessage();
        receivedMessage.ShouldBeOfType<ConfirmGameJoinedMessage>();
        ((ConfirmGameJoinedMessage)receivedMessage).GameState.ShouldBe(GameSessionManager.GameStates.Lobby);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /// <summary>
    /// Test that all clients receive notification that the game session is ready to launch
    /// </summary>
    [Test]
    public void PlayersGetsNotificationWhenGameSessionIsReadyToLaunch()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);
        var testPlayer3Data = new PlayerData(TestPlayer3UserName);
        var testPlayer4Data = new PlayerData(TestPlayer4UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(
          testPlayer1Data,
          testPlayer2Data,
          testPlayer3Data,
          testPlayer4Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddPlayerCardRepository(mockPlayerCardRepository)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        // Act & Assert
        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameSessionReadyToLaunchMessage);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /// <summary>
    /// All players receive player cards for all players in game session.
    /// </summary>
    [Test]
    public void AllClientsReceivePlayerCardsForAllClientsInGameSession()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);
        var testPlayer3Data = new PlayerData(TestPlayer3UserName);
        var testPlayer4Data = new PlayerData(TestPlayer4UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(
          testPlayer1Data,
          testPlayer2Data,
          testPlayer3Data,
          testPlayer4Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddPlayerCardRepository(mockPlayerCardRepository)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        // Act
        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameSessionReadyToLaunchMessage);

        // Assert
        testPlayer1.ContainMessagesInOrder(1,
          new PlayerDataReceivedMessage(testPlayer2Data),
          new PlayerDataReceivedMessage(testPlayer3Data),
          new PlayerDataReceivedMessage(testPlayer4Data));

        testPlayer2.ContainMessagesInOrder(1,
          new PlayerDataReceivedMessage(testPlayer1Data),
          new PlayerDataReceivedMessage(testPlayer3Data),
          new PlayerDataReceivedMessage(testPlayer4Data));

        testPlayer3.ContainMessagesInOrder(1,
          new PlayerDataReceivedMessage(testPlayer1Data),
          new PlayerDataReceivedMessage(testPlayer2Data),
          new PlayerDataReceivedMessage(testPlayer4Data));

        testPlayer4.ContainMessagesInOrder(1,
          new PlayerDataReceivedMessage(testPlayer1Data),
          new PlayerDataReceivedMessage(testPlayer2Data),
          new PlayerDataReceivedMessage(testPlayer3Data));
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void AllClientsReceiveBoardDataWhenGameSessionIsLaunched()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);
        var testPlayer3Data = new PlayerData(TestPlayer3UserName);
        var testPlayer4Data = new PlayerData(TestPlayer4UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(
          testPlayer1Data,
          testPlayer2Data,
          testPlayer3Data,
          testPlayer4Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
                  .AddPlayerCardRepository(mockPlayerCardRepository)
                  .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        var gameInitializationData = GameInitializationDataBuilder.Build(new Board(BoardSizes.Standard));
        var expectedMessage = new InitializeGameMessage(gameInitializationData);

        // Act
        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameInitializationMessage);
        
        // Assert
        testPlayer1.GetLastMessage().IsSameAs(expectedMessage);
        testPlayer2.GetLastMessage().IsSameAs(expectedMessage);
        testPlayer3.GetLastMessage().IsSameAs(expectedMessage);
        testPlayer4.GetLastMessage().IsSameAs(expectedMessage);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void ClientReceivesPersonalMessageFromAnotherClientBeforeGameSessionIsLaunched()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(
          testPlayer1Data,
          testPlayer2Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
                  .AddPlayerCardRepository(mockPlayerCardRepository)
                  .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);

        var testScript = new TestScript(testPlayer1, testPlayer2);
        testScript.AllClientsJoinGame();
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlayerDataReceivedMessage), testPlayer1, testPlayer2);

        var messageText = "Hello There";
        var expectedMessage = new PersonalMessage(TestPlayer1UserName, messageText);

        // Act
        testPlayer1.SendPersonalMessage(messageText);

        // Assert
        testScript.WaitUntilClientsReceiveMessage(expectedMessage, testPlayer2);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void ClientsReceivePersonalMessageFromClientOnceGameSessionIsLaunched()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);
        var testPlayer3Data = new PlayerData(TestPlayer3UserName);
        var testPlayer4Data = new PlayerData(TestPlayer4UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(
          testPlayer1Data,
          testPlayer2Data,
          testPlayer3Data,
          testPlayer4Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
                  .AddPlayerCardRepository(mockPlayerCardRepository)
                  .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameInitializationMessage);

        var messageText = "Hello There";
        var expectedMessage = new PersonalMessage(TestPlayer1UserName, messageText);

        // Act
        testPlayer1.SendPersonalMessage(messageText);

        // Assert
        testScript.WaitUntilClientsReceiveMessage(expectedMessage, testPlayer2, testPlayer3, testPlayer4);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void AllClientsReceiveSameGameTokenWhenJoinedToSameGame()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        // Arrange
        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);
        var testPlayer3Data = new PlayerData(TestPlayer3UserName);
        var testPlayer4Data = new PlayerData(TestPlayer4UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(
          testPlayer1Data,
          testPlayer2Data,
          testPlayer3Data,
          testPlayer4Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
                  .AddPlayerCardRepository(mockPlayerCardRepository)
                  .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        // Act
        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameSessionReadyToLaunchMessage);

        // Assert
        testPlayer1.GameToken.ShouldNotBe(Guid.Empty);
        (testPlayer1.GameToken == testPlayer2.GameToken &&
         testPlayer2.GameToken == testPlayer3.GameToken &&
         testPlayer3.GameToken == testPlayer4.GameToken).ShouldBeTrue();
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void SameClientInterfaceCannotBeAddedToSameGameSession()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        Guid expectedSessionToken = Guid.NewGuid();
        Guid otherSessionToken = Guid.NewGuid();

        var mockGameSessionTokenFactory = NSubstitute.Substitute.For<IGameSessionTokenFactory>();
        mockGameSessionTokenFactory.CreateGameSessionToken().Returns(expectedSessionToken, otherSessionToken);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddGameSessionTokenFactory(mockGameSessionTokenFactory)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);

        // Act- Attempt to add the same client twice.
        var testScript = new TestScript(testPlayer1, testPlayer1);
        testScript.AllClientsJoinGame();
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(ConfirmGameJoinedMessage), testPlayer1);

        // Assert
        testPlayer1.GameToken.ShouldBe(expectedSessionToken);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void SameClientCannotBeAddedAsLastMemberOfGameSession()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        Guid expectedSessionToken = Guid.NewGuid();
        Guid otherSessionToken = Guid.NewGuid();

        var mockGameSessionTokenFactory = NSubstitute.Substitute.For<IGameSessionTokenFactory>();
        mockGameSessionTokenFactory.CreateGameSessionToken().Returns(expectedSessionToken, otherSessionToken);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddGameSessionTokenFactory(mockGameSessionTokenFactory)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);

        // Act - Attempt to add the same client as the last member to the game session.
        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer1);
        testScript.AllClientsJoinGame();
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlayerDataReceivedMessage), testPlayer1, testPlayer2, testPlayer3);

        // Assert
        testPlayer1.GameToken.ShouldBe(expectedSessionToken);
        testPlayer2.GameToken.ShouldBe(expectedSessionToken);
        testPlayer3.GameToken.ShouldBe(expectedSessionToken);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void SameClientCannotBeAddedToAnotherGameSession()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        Guid firstSessionToken = Guid.NewGuid();
        Guid secondSessionToken = Guid.NewGuid();

        var mockGameSessionTokenFactory = NSubstitute.Substitute.For<IGameSessionTokenFactory>();
        mockGameSessionTokenFactory.CreateGameSessionToken().Returns(firstSessionToken, secondSessionToken);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddGameSessionTokenFactory(mockGameSessionTokenFactory)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        // Act - Attempt to add the same client into two game sessions.
        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4, testPlayer1);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameSessionReadyToLaunchMessage);
        //this.WaitUntilClientsReceiveMessageOfType(typeof(GameSessionReadyToLaunchMessage), testPlayer1, testPlayer2, testPlayer3, testPlayer4);

        // Assert
        testPlayer1.GameToken.ShouldBe(firstSessionToken);
        testPlayer2.GameToken.ShouldBe(firstSessionToken);
        testPlayer3.GameToken.ShouldBe(firstSessionToken);
        testPlayer4.GameToken.ShouldBe(firstSessionToken);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void WhenNamedClientDropsOutOfGameSessionBeforeLaunchOtherClientsAreNotified()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var expectedMessageForTestPlayer1 = new PlayerHasLeftGameMessage();
        var expectedMessageForTestPlayer2 = new OtherPlayerHasLeftGameMessage(TestPlayer1UserName);

        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        
        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(testPlayer1Data);
        
        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddPlayerCardRepository(mockPlayerCardRepository)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);

        var testScript = new TestScript(testPlayer1, testPlayer2);
        testScript.AllClientsJoinGame();
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlayerDataReceivedMessage), testPlayer1, testPlayer2);

        // Act
        testPlayer1.LeaveGame();

        // Assert
        testScript.WaitUntilClientsReceiveMessage(expectedMessageForTestPlayer1, testPlayer1);
        testScript.WaitUntilClientsReceiveMessage(expectedMessageForTestPlayer2, testPlayer2);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
    }

    [Test]
    public void WhenNamedClientDropsOutOfGameSessionAfterLaunchOtherClientsAreNotified()
    {
      GameSessionManager gameSessionManager = null;
      try
      {
        var expectedMessageForTestPlayer1 = new PlayerHasLeftGameMessage();
        var expectedMessageForOtherTestPlayers = new OtherPlayerHasLeftGameMessage(TestPlayer1UserName);

        var testPlayer1Data = new PlayerData(TestPlayer1UserName);
        var testPlayer2Data = new PlayerData(TestPlayer2UserName);
        var testPlayer3Data = new PlayerData(TestPlayer3UserName);
        var testPlayer4Data = new PlayerData(TestPlayer4UserName);

        var mockPlayerCardRepository = this.CreateMockPlayerCardRepository(testPlayer1Data, testPlayer2Data, testPlayer3Data, testPlayer4Data);

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddPlayerCardRepository(mockPlayerCardRepository)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayer1 = new TestClient(TestPlayer1UserName, gameSessionManager);
        var testPlayer2 = new TestClient(TestPlayer2UserName, gameSessionManager);
        var testPlayer3 = new TestClient(TestPlayer3UserName, gameSessionManager);
        var testPlayer4 = new TestClient(TestPlayer4UserName, gameSessionManager);

        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilClientsReceiveGameInitializationMessage);

        // Act
        testPlayer1.LeaveGame();

        // Assert
        testScript.WaitUntilClientsReceiveMessage(expectedMessageForTestPlayer1, testPlayer1);
        testScript.WaitUntilClientsReceiveMessage(expectedMessageForOtherTestPlayers, testPlayer2, testPlayer3, testPlayer4);
      }
      finally
      {
        gameSessionManager?.WaitUntilGameSessionManagerHasStopped();
      }
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

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(4)
          .AddGameManagerFactory(mockGameManagerFactory)
          .WaitUntilGameSessionManagerHasStarted();

        var testPlayers = new[]
        {
          new TestClient(TestPlayer1UserName, gameSessionManager),
          new TestClient(TestPlayer2UserName, gameSessionManager),
          new TestClient(TestPlayer3UserName, gameSessionManager),
          new TestClient(TestPlayer4UserName, gameSessionManager)
        };

        var testPlayer1 = testPlayers[0];
        var testPlayer2 = testPlayers[1];
        var testPlayer3 = testPlayers[2];
        var testPlayer4 = testPlayers[3];

        var firstTestPlayer = testPlayers[firstSetupPassOrder[0]];
        var secondTestPlayer = testPlayers[firstSetupPassOrder[1]];
        var thirdTestPlayer = testPlayers[firstSetupPassOrder[2]];
        var fourthTestPlayer = testPlayers[firstSetupPassOrder[3]];

        var testScript = new TestScript(testPlayer1, testPlayer2, testPlayer3, testPlayer4);
        testScript.RunUntil(TestScript.RunPoints.RunUntilEnd);
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlaceTownMessage), firstTestPlayer);
        testScript.SendTownPlacementFromClient(firstTestPlayer, 0u);
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlaceTownMessage), secondTestPlayer);
        testScript.SendTownPlacementFromClient(secondTestPlayer, 10u);
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlaceTownMessage), thirdTestPlayer);
        testScript.SendTownPlacementFromClient(thirdTestPlayer, 20u);
        testScript.WaitUntilClientsReceiveMessageOfType(typeof(PlaceTownMessage), fourthTestPlayer);
      }
      finally
      {
        gameSessionManager.WaitUntilGameSessionManagerHasStopped();
      }
    }

    /*
    /// <summary>
    /// If a client sends multiple game initialization confirmation messages then the
    /// subsequent messages should be ignored. In this scenario the second client 
    /// sends nothing so the same number of messages are being sent to the server.
    /// </summary>
    [Test]
    public void SubsequentGameInitializationConfirminationMessagesAreIgnored()
    {
      // Arrange
      var gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

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
    [Test]
    public void WhenWaitingForGameInitializationConfirminationMessagesWrongMessagesAreIgnored()
    {
      // Arrange
      var gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(new GameManagerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

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

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

        var mockClient1 = new MockClient(gameSessionManager);
        var mockClient2 = new MockClient(gameSessionManager);
        var mockClient3 = new MockClient(gameSessionManager);
        var mockClient4 = new MockClient(gameSessionManager);

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

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

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

    [Test]
    [TestCase(new UInt32[] { 0u, 1u, 2u, 3u }, new UInt32[] { 1u, 5u, 13u, 27u, 3u, 7u, 17u, 30u })]
    [TestCase(new UInt32[] { 3u, 0u, 1u, 2u }, new UInt32[] { 15u, 3u, 12u, 36u, 11u, 6u, 22u, 30u })]
    [TestCase(new UInt32[] { 2u, 3u, 0u, 1u }, new UInt32[] { 10u, 2u, 14u, 23u, 33u, 9u, 21u, 37u })]
    [TestCase(new UInt32[] { 1u, 2u, 3u, 0u }, new UInt32[] { 34u, 26u, 16u, 20u, 34u, 8u, 0u, 40u })]
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

        gameSessionManager = GameSessionManagerTestExtensions.CreateGameSessionManagerForTest(mockGameManagerFactory, 4);

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

    private void ConfirmGameInitializedForClients(params MockClient[] mockClients)
    {
      foreach (var mockClient in mockClients)
      {
        mockClient.ConfirmGameInitialized();
      }
    }

    private void WaitUntilClientsReceiveGameData(params MockClient[] mockClients)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      var waitingForGameData = new List<MockClient>(mockClients);

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
    }*/

    private IPlayerCardRepository CreateMockPlayerCardRepository(PlayerData mandatoryPlayerData, params PlayerData[] playerDataList)
    {
      var mockPlayerCardRepository = Substitute.For<IPlayerCardRepository>();
      mockPlayerCardRepository.GetPlayerData(mandatoryPlayerData.Username).Returns(mandatoryPlayerData);
      foreach (var playerData in playerDataList)
      {
        mockPlayerCardRepository.GetPlayerData(playerData.Username).Returns(playerData);
      }

      return mockPlayerCardRepository;
    }
    #endregion
  }
}
