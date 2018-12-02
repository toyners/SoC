
namespace Jabberwocky.SoC.Library.UnitTests
{
    using System;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.PlayerData;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class GameControllerFactory_UnitTests
    {
        #region Methods
        [Test]
        public void Create_NullGameControllerSetupParameter_ThrowsMeaningfulException()
        {
            var exception = Should.Throw<ArgumentNullException>(() =>
            {
                new GameControllerFactory().Create(null, null);
            });

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentNullException>();
            exception.InnerException.ShouldBeNull();
            exception.Message.ShouldBe("Parameter 'gameControllerSetup' is null.");
        }

        [Test]
        public void Create_SetupWithMissingHandlers_ThrowsMeaningfulException()
        {
            var exception = Should.Throw<NullReferenceException>(() =>
            {
                new GameControllerFactory().Create(null, new GameControllerSetup());
            });

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<NullReferenceException>();
            exception.InnerException.ShouldBeNull();
            exception.Message.ShouldBe("The following Event Handlers are not set: GameJoinedEventHandler, InitialBoardSetupEventHandler, LoggedInEventHandler, StartInitialSetupTurnEvent");
        }

        [Test]
        public void Create_SetupWithAllHandlers_ControllerHasHandlersSet()
        {
            var gameControllerSetup = this.CreateGameControllerSetup();
            var gameController = new GameControllerFactory().Create(null, gameControllerSetup);

            gameController.GameJoinedEvent.ShouldBe(gameControllerSetup.GameJoinedEventHandler);
            gameController.InitialBoardSetupEvent.ShouldBe(gameControllerSetup.InitialBoardSetupEventHandler);
            gameController.LoggedInEvent.ShouldBe(gameControllerSetup.LoggedInEventHandler);
            gameController.StartInitialSetupTurnEvent.ShouldBe(gameControllerSetup.StartInitialSetupTurnEvent);
        }

        [Test]
        public void Create_NullParameter_ReturnsLocalController()
        {
            var gameController = new GameControllerFactory().Create(null, this.CreateGameControllerSetup());
            gameController.ShouldBeOfType<LocalGameController>();
        }

        [Test]
        public void Create_DefaultGameOptions_ReturnsLocalController()
        {
            var gameController = new GameControllerFactory().Create(new GameOptions(), this.CreateGameControllerSetup());
            gameController.ShouldBeOfType<LocalGameController>();
        }

        [Test]
        public void Create_GameOptionsSetToLocalConnection_ReturnsLocalController()
        {
            var gameController = new GameControllerFactory().Create(new GameOptions { Connection = Enums.GameConnectionTypes.Local }, this.CreateGameControllerSetup());
            gameController.ShouldBeOfType<LocalGameController>();
        }

        private GameControllerSetup CreateGameControllerSetup()
        {
            return new GameControllerSetup
            {
                GameJoinedEventHandler = (PlayerDataBase[] players) => { },
                InitialBoardSetupEventHandler = (GameBoardSetup g) => { },
                LoggedInEventHandler = (ClientAccount clientAccount) => { },
                StartInitialSetupTurnEvent = (GameBoards.GameBoardUpdate gameBoardUpdate) => { }
            };
        }
        #endregion
    }
}
