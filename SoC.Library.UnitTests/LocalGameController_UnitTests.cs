
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class LocalGameController_UnitTests
  {
    #region Methods
    [Test]
    public void StartJoiningGame_DefaultLocalGame_GameJoinedForOnePlayerWithAIPlayersInOtherSlots()
    {
      var localGameController = new LocalGameController();

      PlayerBase[] players = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.StartJoiningGame(null);

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerView>();
      players[2].ShouldBeOfType<PlayerView>();
      players[3].ShouldBeOfType<PlayerView>();
    }

    [Test]
    [TestCase(1, 3)]
    public void StartJoiningGame_LocalGameWithExplicitGameFilter_GameJoinedWithPlayerSlotsFilled(Int32 maxPlayers, Int32 maxAIPlayers)
    {
      var localGameController = new LocalGameController();

      PlayerBase[] players = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.StartJoiningGame(new GameFilter { MaxPlayers = (UInt32)maxPlayers, MaxAIPlayers = (UInt32)maxAIPlayers });

      players.ShouldNotBeNull();
      players.Length.ShouldBe(maxPlayers + maxAIPlayers);

      var index = 0;

      while (maxPlayers-- > 0)
      {
        players[index++].ShouldBeOfType<PlayerData>();
      }

      while (maxAIPlayers-- > 0)
      {
        players[index++].ShouldBeOfType<PlayerView>();
      }
    }
    #endregion 
  }
}
