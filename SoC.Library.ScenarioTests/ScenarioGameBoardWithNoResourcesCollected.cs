
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;

    public class ScenarioGameBoardWithNoResourcesCollected : GameBoard
    {
        public ScenarioGameBoardWithNoResourcesCollected() : base(BoardSizes.Standard) { }

        public override ResourceClutch GetResourcesForLocation(uint location)
        {
            return ResourceClutch.Zero;
        }

        public override Dictionary<Guid, ResourceCollection[]> GetResourcesForRoll(uint diceRoll)
        {
            return new Dictionary<Guid, ResourceCollection[]>();
        }
    }
}
