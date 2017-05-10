
namespace SoC.Library.IntegrationTests
{
  using System;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Enums;
  using Jabberwocky.SoC.Library.Interfaces;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Empty_Test_Class1
  {
    #region Methods
    [Test]
    public void JoinEmptyGame()
    {
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(GameConnectionTypes.Local);
      Player player = null;
      gameController.GameJoinedEvent = (Player p) => { player = p; };

      gameController.StartJoiningGame(null);

      player.ShouldNotBeNull();
    }
    #endregion 
  }
}
