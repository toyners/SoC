﻿
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
    private Int32 maximumPlayerCount;
    private IDiceRollerFactory diceRollerFactory;
    #endregion

    #region Construction
    public GameSessionManager(IDiceRollerFactory diceRollerFactory, Int32 maximumPlayerCount = 1)
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGame = new Queue<IServiceProviderCallback>();
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
      this.waitingForGame.Enqueue(client);
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

    private void MatchPlayersWithGames()
    {
      this.working = true;
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

        if (!matchMade)
        {
          // Create a new game and add the player
          client = this.waitingForGame.Dequeue();
          gameSession = new GameSession(this.diceRollerFactory.Create(), this.maximumPlayerCount);
          gameSession.AddPlayer(client);
          this.gameSessions.Add(gameSession.GameToken, gameSession);
        }

        if (!gameSession.NeedsPlayer)
        {
          // Game is full so initialise the game here 
          // and all clients
          gameSession.InitializeGame();
        }
      }
    }
    #endregion

    #region Classes
    private class GameSession
    {
      #region Fields
      public GameManager Game;

      public Player[] Players;

      public IServiceProviderCallback[] Clients;

      public Guid GameToken;

      private Board board;

      private Int32 clientCount;
      #endregion

      #region Construction
      public GameSession(IDiceRoller diceRoller, Int32 maximumPlayerCount)
      {
        this.GameToken = Guid.NewGuid();

        this.Players = new Player[maximumPlayerCount];
        this.Clients = new IServiceProviderCallback[maximumPlayerCount];

        this.board = new Board(BoardSizes.Standard);
        this.Game = new GameManager(this.board, diceRoller, this.Players, new DevelopmentCardPile());
      }
      #endregion

      #region Properties
      public Boolean NeedsPlayer
      {
        get { return this.clientCount < this.Clients.Length; }
      }
      #endregion

      #region Methods
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
            client.ConfirmGameLeft();
            return;
          }
        }

        throw new NotImplementedException();
      }

      public void InitializeGame()
      {
        var gameData = GameInitializationDataBuilder.Build(this.board);
        foreach (var client in this.Clients)
        {
          client.GameInitialization(gameData);
        }


      }
      #endregion
    }
    #endregion
  }
}
