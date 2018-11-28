
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System;
    using GameBoards;
    using Jabberwocky.SoC.Library.PlayerData;

    public interface IGameController
    {
        #region Events
        Action<GameBoardUpdate> BoardUpdatedEvent { get; set; }
        Action<PlayerDataBase[]> GameJoinedEvent { get; set; }
        Action<GameBoard> InitialBoardSetupEvent { get; set; }
        Action<ClientAccount> LoggedInEvent { get; set; }
        Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
        #endregion

        #region Properties
        Guid GameId { get; }
        #endregion

        #region Methods
        void JoinGame(GameOptions gameOptions);
        void Quit();
        #endregion
    }

    public struct Offer
    {
        PlayerDataOld player;

        Int32 OfferedOre;
        Int32 OfferedWheat;
        Int32 OfferedSheep;
        Int32 OfferedLumber;
        Int32 OfferedBrick;

        Int32 WantedOreCount;
        Int32 WantedWheatCount;
        Int32 WantedSheepCount;
        Int32 WantedLumberCount;
        Int32 WantedBrickCount;
    }
}
