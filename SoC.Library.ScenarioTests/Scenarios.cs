
namespace SoC.Library.ScenarioTests
{
    public class Scenarios
    {
        const string MainPlayer = "Player";
        const string FirstOpponent_Babara = "Barbara";
        const string SecondOpponent_Charlie = "Charlie";
        const string ThirdOpponent_Dana = "Dana";

        const uint MainPlayerFirstSettlementLocation = 12u;
        const uint FirstOpponentFirstSettlementLocation = 18u;
        const uint SecondOpponentFirstSettlementLocation = 25u;
        const uint ThirdOpponentFirstSettlementLocation = 31u;

        const uint ThirdOpponentSecondSettlementLocation = 33u;
        const uint SecondOpponentSecondSettlementLocation = 35u;
        const uint FirstOpponentSecondSettlementLocation = 43u;
        const uint MainPlayerSecondSettlementLocation = 40u;

        const uint MainPlayerFirstRoadEnd = 4;
        const uint FirstOpponentFirstRoadEnd = 17;
        const uint SecondOpponentFirstRoadEnd = 15;
        const uint ThirdOpponentFirstRoadEnd = 30;

        const uint ThirdOpponentSecondRoadEnd = 32;
        const uint SecondOpponentSecondRoadEnd = 24;
        const uint FirstOpponentSecondRoadEnd = 44;
        const uint MainPlayerSecondRoadEnd = 39;

        [Scenario]
        public void Scenario_AllPlayersCompleteSetup(string[] args)
        {
            this.CreateStandardLocalGameControllerScenarioRunner(args)
                .WithNoResourceCollection()
                .InitialBoardSetupEvent()
                .PlayerSetupTurn(MainPlayer, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd)
                .PlayerSetupTurn(FirstOpponent_Babara, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd)
                .PlayerSetupTurn(SecondOpponent_Charlie, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd)
                .PlayerSetupTurn(ThirdOpponent_Dana, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd)
                .PlayerSetupTurn(ThirdOpponent_Dana, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .PlayerSetupTurn(SecondOpponent_Charlie, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .PlayerSetupTurn(FirstOpponent_Babara, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .PlayerSetupTurn(MainPlayer, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .Run();
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner(string[] args)
        {
            return LocalGameControllerScenarioRunner.LocalGameController(args)
                .WithHumanPlayer(MainPlayer)
                .WithComputerPlayer2(FirstOpponent_Babara)
                .WithComputerPlayer2(SecondOpponent_Charlie)
                .WithComputerPlayer2(ThirdOpponent_Dana)
                .WithTurnOrder(MainPlayer, FirstOpponent_Babara, SecondOpponent_Charlie, ThirdOpponent_Dana);
        }
    }
}
