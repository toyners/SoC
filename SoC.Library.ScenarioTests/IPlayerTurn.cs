using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    public interface IPlayerTurn
    {
        IPlayerTurn BuildCity(uint cityLocation);
        IPlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd);
        IPlayerTurn BuildSettlement(uint settlementLocation);
        IPlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType);
        IPlayerTurn PlayKnightCard();
    }
}