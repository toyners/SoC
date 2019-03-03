
namespace SoC.Library.ScenarioTests
{
    public class Scenarios
    {
        const string Adam = "Adam";
        const string Babara = "Barbara";
        const string Charlie = "Charlie";
        const string Dana = "Dana";

        const uint Adam_FirstSettlementLocation = 12u;
        const uint Babara_FirstSettlementLocation = 18u;
        const uint Charlie_FirstSettlementLocation = 25u;
        const uint Dana_FirstSettlementLocation = 31u;

        const uint Dana_SecondSettlementLocation = 33u;
        const uint Charlie_SecondSettlementLocation = 35u;
        const uint Babara_SecondSettlementLocation = 43u;
        const uint Adam_SecondSettlementLocation = 40u;

        const uint Adam_FirstRoadEnd = 4;
        const uint Babara_FirstRoadEnd = 17;
        const uint Charlie_FirstRoadEnd = 15;
        const uint Dana_FirstRoadEnd = 30;

        const uint Dana_SecondRoadEnd = 32;
        const uint Charlie_SecondRoadEnd = 24;
        const uint Babara_SecondRoadEnd = 44;
        const uint Adam_SecondRoadEnd = 39;

        [Scenario]
        public void Scenario_AllPlayersCompleteSetup(string[] args)
        {
            this.CreateStandardLocalGameControllerScenarioRunner(args)
                .WithNoResourceCollection()
                .PlayerSetupEvent()
                .InitialBoardSetupEvent()
                .PlayerSetupTurn(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEnd)
                .PlayerSetupTurn(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEnd)
                .PlayerSetupTurn(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEnd)
                .PlayerSetupTurn(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEnd)
                .PlayerSetupTurn(Dana, Dana_SecondSettlementLocation, Dana_SecondRoadEnd)
                .PlayerSetupTurn(Charlie, Charlie_SecondSettlementLocation, Charlie_SecondRoadEnd)
                .PlayerSetupTurn(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEnd)
                .PlayerSetupTurn(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEnd)
                .Run();
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner(string[] args)
        {
            return LocalGameControllerScenarioRunner.LocalGameController(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(Adam, Babara, Charlie, Dana);
        }
    }
}
