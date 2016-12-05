
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

  public class GameSessionManager
  {
    #region Fields
    private Boolean working;
    private List<IServiceProviderCallback> clients;
    private Dictionary<Guid, GameSession> gameSessions;
    private ConcurrentQueue<IServiceProviderCallback> waitingForGameQueue;
    private Task matchingTask;
    private UInt32 maximumPlayerCount;
    private IDiceRollerFactory diceRollerFactory;
    #endregion

    #region Construction
    public GameSessionManager(IDiceRollerFactory diceRollerFactory, UInt32 maximumPlayerCount = 1)
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGameQueue = new ConcurrentQueue<IServiceProviderCallback>();
      this.gameSessions = new Dictionary<Guid, GameSession>();
      this.maximumPlayerCount = maximumPlayerCount;
      this.diceRollerFactory = diceRollerFactory;
    }
    #endregion

    #region Properties
    public Boolean CanStop
    {
      get { return this.working; /* TODO: Check task and task status */ }
    }

    public Boolean CanStart
    {
      get { return !this.working; /* TODO: Check task and task status */ }
    }

    public Boolean IsProcessing { get { return this.working; } }
    #endregion

    #region Methods
    public void AddClient(IServiceProviderCallback client)
    {
      // TODO: Check for null reference
      this.waitingForGameQueue.Enqueue(client);
    }

    public void RemoveClient(Guid gameToken, IServiceProviderCallback client)
    {
      if (!this.gameSessions.ContainsKey(gameToken))
      {
        throw new NotImplementedException();
      }

      var gameSession = this.gameSessions[gameToken];
      gameSession.RemoveClient(client);
    }

    public void StopMatching()
    {
      if (!this.CanStop)
      {
        return;
      }

      this.working = false;
    }

    public void StartMatching()
    {
      if (!this.CanStart)
      {
        return;
      }

      this.matchingTask = Task.Factory.StartNew(() => { this.MatchPlayersWithGames(); });
    }

    private GameSession AddToNewGameSession(IServiceProviderCallback client)
    {
      var gameSession = new GameSession(this.diceRollerFactory.Create(), this.maximumPlayerCount);
      gameSession.AddClient(client);
      this.gameSessions.Add(gameSession.GameToken, gameSession);
      return gameSession;
    }

    private void MatchPlayersWithGames()
    {
      this.working = true;
      while (this.working)
      {
        while (this.waitingForGameQueue.IsEmpty)
        {
          Thread.Sleep(500);
        }

        IServiceProviderCallback client;
        var gotClient = this.waitingForGameQueue.TryDequeue(out client);
        if (!gotClient)
        {
          // Couldn't get the client from the queue (probably because another thread got it).
          continue;
        }

        GameSession gameSession = null;
        if (!this.TryAddToCurrentGameSession(client, out gameSession))
        {
          gameSession = this.AddToNewGameSession(client);
        }

        if (!gameSession.NeedsClient)
        {
          // Game is full so start it
          gameSession.StartGame();
        }
      }
    }

    private Boolean TryAddToCurrentGameSession(IServiceProviderCallback client, out GameSession gameSession)
    {
      gameSession = null;
      foreach (var kv in this.gameSessions)
      {
        gameSession = kv.Value;
        if (gameSession.NeedsClient)
        {
          gameSession.AddClient(client);
          return true;
        }
      }

      return false;
    }
    #endregion

    #region Classes
    private class GameSession
    {
      #region Fields
      public GameManager Game;

      public IServiceProviderCallback[] Clients;

      public Guid GameToken;

      private Board board;

      private Int32 clientCount;

      private Task gameTask;
      #endregion

      #region Construction
      public GameSession(IDiceRoller diceRoller, UInt32 playerCount)
      {
        this.GameToken = Guid.NewGuid();

        this.Clients = new IServiceProviderCallback[playerCount];

        this.board = new Board(BoardSizes.Standard);
        this.Game = new GameManager(this.board, diceRoller, playerCount, new DevelopmentCardPile());
      }
      #endregion

      #region Properties
      public Boolean NeedsClient
      {
        get { return this.clientCount < this.Clients.Length; }
      }
      #endregion

      #region Methods
      public void AddClient(IServiceProviderCallback client)
      {
        for (var i = 0; i < this.Clients.Length; i++)
        {
          if (this.Clients[i] == null)
          {
            var player = new Player(this.board);
            this.Clients[i] = client;
            this.clientCount++;
            client.ConfirmGameJoined(this.GameToken);
            return;
          }
        }

        throw new NotImplementedException();
      }

      public void RemoveClient(IServiceProviderCallback client)
      {
        for (Int32 i = 0; i < this.Clients.Length; i++)
        {
          if (this.Clients[i] == client)
          {
            this.Clients[i] = null;
            this.clientCount--;
            client.ConfirmGameLeft();
            return;
          }
        }

        throw new NotImplementedException();
      }

      public void StartGame()
      {
        this.gameTask = Task.Factory.StartNew(() =>
        {
          var gameData = GameInitializationDataBuilder.Build(this.board);
          foreach (var client in this.Clients)
          {
            client.InitializeGame(gameData);
          }

          //TODO: wait for initialization confirmed replies from clients

          var playerIndexes = this.Game.GetFirstSetupPassOrder();
          var waitingForResponse = true;
          foreach (var playerIndex in playerIndexes)
          {
            var client = this.Clients[playerIndex];
            client.PlaceTown();

            // Wait until response from client
            while (waitingForResponse)
            {
              Thread.Sleep(50);
            }
          }

        });
      }
      #endregion
    }

    private class LockableQueue
    {
      private static Object queueLock;
    }
    #endregion
  }
}
