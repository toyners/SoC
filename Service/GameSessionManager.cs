
namespace Jabberwocky.SoC.Service
{
  using System;
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
    private Queue<IServiceProviderCallback> waitingForGame;
    private Task matchingTask;
    #endregion

    #region Construction
    public GameSessionManager()
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGame = new Queue<IServiceProviderCallback>();
      this.gameSessions = new Dictionary<Guid, GameSession>();
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

    public void RemoveClient(Guid gameToken, IServiceProviderCallback client)
    {
      if (!this.gameSessions.ContainsKey(gameToken))
      {
        throw new NotImplementedException();
      }

      var gameSession = this.gameSessions[gameToken];
      gameSession.RemovePlayer(client);
    }
    #endregion

    #region Methods
    public void AddClient(IServiceProviderCallback client)
    {
      // TODO: Check for null reference
      this.waitingForGame.Enqueue(client);
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
      this.working = true;
    }

    private void MatchPlayersWithGames()
    {
      while (this.working)
      {
        while (this.waitingForGame.Count == 0)
        {
          Thread.Sleep(500);
        }

        IServiceProviderCallback client;
        var matchMade = false;
        GameSession gameSession = null;
        foreach (var kv in this.gameSessions)
        {
          gameSession = kv.Value;
          if (gameSession.NeedsPlayer)
          {
            client = this.waitingForGame.Dequeue();
            gameSession.AddPlayer(client);
            matchMade = true;
            break;
          }
        }

        if (matchMade)
        {
          if (!gameSession.NeedsPlayer)
          {
            // Game is full so initialise the game here 
            // and all clients
            gameSession.InitializeGame();
          }

          continue;
        }

        // Create a new game and add the player
        client = this.waitingForGame.Dequeue();
        gameSession = new GameSession();
        gameSession.AddPlayer(client);
        this.gameSessions.Add(gameSession.GameToken, gameSession);
      }
    }
    #endregion

    private class GameSession
    {
      public GameManager Game;

      public Player[] Players;

      public IServiceProviderCallback[] Clients;

      public Guid GameToken;

      private Board board;

      private Int32 clientCount;

      public GameSession()
      {
        this.GameToken = Guid.NewGuid();
        this.board = new Board();
        var diceRoller = new DiceRoller();
        this.Game = new GameManager(this.board, diceRoller, this.Players, new DevelopmentCardPile());

        this.Players = new Player[4];
        this.Clients = new IServiceProviderCallback[4];
      }

      public Boolean NeedsPlayer
      {
        get { return this.clientCount < this.Clients.Length; }
      }

      public void AddPlayer(IServiceProviderCallback client)
      {
        for (var i = 0; i < this.Clients.Length; i++)
        {
          if (this.Clients[i] == null)
          {
            var player = new Player(this.board);
            this.Clients[i] = client;
            this.Players[i] = player;
            this.clientCount++;
            client.ConfirmGameJoined(this.GameToken);
            return;
          }
        }

        throw new NotImplementedException();
      }

      public void RemovePlayer(IServiceProviderCallback client)
      {
        for (Int32 i = 0; i < this.Clients.Length; i++)
        {
          if (this.Clients[i] == client)
          {
            this.Clients[i] = null;
            this.clientCount--;
            client.ConfirmGameLeft(this.GameToken);
            return;
          }
        }

        throw new NotImplementedException();
      }

      public void InitializeGame()
      {
        foreach (var client in this.Clients)
        {
          client.GameInitialization();
        }
      }
    }
  }
}
