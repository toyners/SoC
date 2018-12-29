
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System.Collections.Generic;
    using GameActions;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.PlayerData;

    public interface IComputerPlayer : IPlayer
    {
        #region Methods
        void AddDevelopmentCard(DevelopmentCard developmentCard);
        void BuildInitialPlayerActions(PlayerDataModel[] playerData);
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
        ComputerPlayerAction GetPlayerAction();
        YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard();
        #endregion
    }
}
