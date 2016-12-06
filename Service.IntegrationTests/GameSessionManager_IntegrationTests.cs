
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;
  using Jabberwocky.Toolkit.Object;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameSessionManager_IntegrationTests
  {
    #region Methods
    [Test]
    public void AddClient_AddPlayerToNonFullSession_PlayerAdded()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory());
      var mockClient = Substitute.For<IServiceProviderCallback>();

      // Act
      gameSessionManager.AddClient(mockClient);
      Thread.Sleep(1000);
      gameSessionManager.StopMatching();

      // Assert
      mockClient.Received().ConfirmGameJoined(Arg.Any<Guid>());
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = Substitute.For<IServiceProviderCallback>();
      var mockClient2 = Substitute.For<IServiceProviderCallback>();
      var mockClient3 = Substitute.For<IServiceProviderCallback>();
      var mockClient4 = Substitute.For<IServiceProviderCallback>();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.StopMatching();

      // Assert
      Guid token = Guid.Empty;
      mockClient1.ConfirmGameJoined(Arg.Is<Guid>(gameToken => gameToken != Guid.Empty));
      mockClient1.ConfirmGameJoined(Arg.Do<Guid>(gameToken => token = gameToken));

      mockClient2.ConfirmGameJoined(Arg.Is<Guid>(gameToken => gameToken == token));
      mockClient3.ConfirmGameJoined(Arg.Is<Guid>(gameToken => gameToken == token));
      mockClient4.ConfirmGameJoined(Arg.Is<Guid>(gameToken => gameToken == token));
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersAreInitialized()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = Substitute.For<IServiceProviderCallback>();
      var mockClient2 = Substitute.For<IServiceProviderCallback>();
      var mockClient3 = Substitute.For<IServiceProviderCallback>();
      var mockClient4 = Substitute.For<IServiceProviderCallback>();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.StopMatching();
      
      // Assert
      mockClient1.Received().InitializeGame(Arg.Is<GameInitializationData>(gameData => gameData != null));
      mockClient2.Received().InitializeGame(Arg.Is<GameInitializationData>(gameData => gameData != null));
      mockClient3.Received().InitializeGame(Arg.Is<GameInitializationData>(gameData => gameData != null));
      mockClient4.Received().InitializeGame(Arg.Is<GameInitializationData>(gameData => gameData != null));
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_FirstPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 12u, 8u, 6u, 4u };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_SecondPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 12u, 6u, 4u };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_ThirdPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 6u, 12u, 4u };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_LastPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 6u, 4u, 12u };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls);
    }

    private void GameIsFullSoPlayerGetsToPlaceFirstTown(List<UInt32> diceRolls)
    {
      // Arrange
      var index = 0;
      var diceRoller = Substitute.For<IDiceRoller>();
      diceRoller.RollTwoDice().Returns(x => { return diceRolls[index++]; });

      var diceRollerFactory = Substitute.For<IDiceRollerFactory>();
      diceRollerFactory.Create().Returns(diceRoller);
        
      var gameSessionManager = this.CreateGameSessionManager(diceRollerFactory, 4);

      var mockClient1 = Substitute.For<IServiceProviderCallback>();
      var mockClient2 = Substitute.For<IServiceProviderCallback>();
      var mockClient3 = Substitute.For<IServiceProviderCallback>();
      var mockClient4 = Substitute.For<IServiceProviderCallback>();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      Guid gameToken = Guid.Empty;
      mockClient1.ConfirmGameJoined(Arg.Is<Guid>(token => token != Guid.Empty));
      mockClient1.ConfirmGameJoined(Arg.Do<Guid>(token => gameToken = token));

      gameSessionManager.ConfirmGameInitialized(gameToken, mockClient1);
      
      gameSessionManager.StopMatching();

      // Assert
      mockClient1.Received().PlaceTown();
      mockClient2.Received().PlaceTown();
      mockClient3.Received().PlaceTown();
      mockClient4.Received().PlaceTown();
    }

    private TestDiceRollerFactory CreateDiceRollerFactory(params List<UInt32>[] numbers)
    {
      var diceRollers = new List<TestDiceRoller>();
      foreach (var numberSet in numbers)
      {
        diceRollers.Add(new TestDiceRoller(numberSet));
      }

      return new TestDiceRollerFactory(diceRollers);
    }

    private GameSessionManager CreateGameSessionManager(IDiceRollerFactory diceRollerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(diceRollerFactory, maximumPlayerCount);
      gameSessionManager.StartMatching();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      while (!gameSessionManager.IsProcessing && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(500);
      }

      stopWatch.Stop();
      if (!gameSessionManager.IsProcessing)
      {
        throw new Exception("GameSessionManager has not started");
      }
      
      return gameSessionManager;
    }

    public class TestDiceRollerFactory : IDiceRollerFactory
    {
      private Queue<TestDiceRoller> diceRollers;

      public TestDiceRollerFactory(List<TestDiceRoller> diceRollers)
      {
        this.diceRollers = new Queue<TestDiceRoller>(diceRollers);
      }

      public IDiceRoller Create()
      {
        if (this.diceRollers.Count == 0)
        {
          throw new Exception("No more dice rollers in dice roller factory.");
        }

        return this.diceRollers.Dequeue();
      }
    }

    public class TestDiceRoller : IDiceRoller
    {
      private Queue<UInt32> numbers;

      public TestDiceRoller(List<UInt32> numbers)
      {
        this.numbers = new Queue<UInt32>(numbers);
      } 

      public UInt32 RollTwoDice()
      {
        if (this.numbers.Count == 0)
        {
          throw new Exception("No more numbers in dice roller.");
        }

        return this.numbers.Dequeue();
      }
    }
    #endregion 
  }
}
