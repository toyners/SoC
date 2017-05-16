
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameControllerFactory_UnitTests
  {
    #region Methods
    [Test]
    public void Create_NullParameter_ReturnsLocalController()
    {
      var gameController = new GameControllerFactory().Create(null);
      gameController.ShouldBeOfType<LocalGameController>();
    }

    [Test]
    public void Create_DefaultGameOptions_ReturnsLocalController()
    {
      var gameController = new GameControllerFactory().Create(new GameOptions());
      gameController.ShouldBeOfType<LocalGameController>();
    }

    [Test]
    public void Create_GameOptionsSetToLocalConnection_ReturnsLocalController()
    {
      var gameController = new GameControllerFactory().Create(new GameOptions { Connection = Enums.GameConnectionTypes.Local });
      gameController.ShouldBeOfType<LocalGameController>();
    }
    #endregion 
  }
}
