
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.PlayerActions;
    using Jabberwocky.SoC.Library.PlayerData;

    public interface IComputerPlayer : IPlayer
    {
        #region Methods
        void AddDevelopmentCard(DevelopmentCard developmentCard);
        void BuildInitialPlayerActions(PlayerDataModel[] playerData, bool moveRobber);
        uint ChooseCityLocation();
        uint ChooseSettlementLocation();
        void ChooseInitialInfrastructure(out uint settlementLocation, out uint roadEndLocation);
        KnightDevelopmentCard GetKnightCard();
        MonopolyDevelopmentCard ChooseMonopolyCard();
        ResourceClutch ChooseResourcesToCollectFromBank();
        ResourceClutch ChooseResourcesToDrop();
        ResourceTypes ChooseResourceTypeToRob();
        uint ChooseRobberLocation();
        IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers);
        PlayerAction PlayTurn(PlayerDataModel[] otherPlayerData, LocalGameController localGameController);
        DropResourcesAction GetDropResourcesAction();
        PlayerAction GetPlayerAction();
        YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard();
        #endregion
    }
}
