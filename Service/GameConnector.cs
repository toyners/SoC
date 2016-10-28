
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

  public class GameConnector
  {
    private Boolean working;
    private List<IServiceProviderCallback> clients;
    private Dictionary<Guid, GameRecord> games;
    private Queue<IServiceProviderCallback> waitingForGame;
    private Task matchingTask;

    public GameConnector()
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGame = new Queue<IServiceProviderCallback>();
      this.games = new Dictionary<Guid, GameRecord>();
    }

    public Boolean CanStop { get { return this.working; /* TODO: Check task and task status */ } }
    public Boolean CanStart { get { return !this.working; /* TODO: Check task and task status */ } }

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

    public void MatchPlayersWithGames()
    {
      while (this.working)
      {
        while (this.waitingForGame.Count == 0)
        {
          Thread.Sleep(500);
        }

        IServiceProviderCallback client;
        GameRecord game;
        var matchMade = false;
        foreach (var kv in this.games)
        {
          game = kv.Value;
          if (game.NeedsPlayer)
          {
            client = this.waitingForGame.Dequeue();
            game.AddPlayer(client);
            matchMade = true;
            break;
          }
        }

        if (matchMade)
        {
          continue;
        }

        // Create a new game and add the player
        client = this.waitingForGame.Dequeue();
        game = new GameRecord();
        game.AddPlayer(client);
        this.games.Add(game.GameToken, game);
      }
    }

    private class GameRecord
    {
      public GameManager Game;

      public Player[] Players;

      public IServiceProviderCallback[] Clients;

      public Guid GameToken;

      private Int32 index = 0;

      public GameRecord()
      {
        this.GameToken = Guid.NewGuid();
        var board = new Board();
        var diceRoller = new DiceRoller();
        this.Game = new GameManager(board, diceRoller, this.Players, new DevelopmentCardPile());

        this.Players = new Player[4];
        this.Clients = new IServiceProviderCallback[4];
      }

      public Boolean NeedsPlayer
      {
        get { return this.index < this.Clients.Length; }
      }

      public void AddPlayer(IServiceProviderCallback client)
      {
        var player = new Player(null);
        this.Players[this.index] = player;
        this.Clients[this.index] = client;
        this.index++;
        client.ConfirmGameJoined(this.GameToken);
      }
    }
  }
}
