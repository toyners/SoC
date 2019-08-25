
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;

    public class ScenarioGameBoard : GameBoard
    {
        public enum ResourceCollectionTypes
        {
            Neither = 0,
            SetupOnly = 1,
        }

        private ResourceCollectionTypes resourceCollectionType = ResourceCollectionTypes.Neither;

        public ScenarioGameBoard(ResourceCollectionTypes resourceCollectionType)
            : base(BoardSizes.Standard) => this.resourceCollectionType = resourceCollectionType;

        public override ResourceClutch GetResourcesForLocation(uint location)
        {
            if (this.resourceCollectionType == ResourceCollectionTypes.SetupOnly)
                return base.GetResourcesForLocation(location);

            return ResourceClutch.Zero;
        }

        public override Dictionary<Guid, ResourceCollection[]> GetResourcesForRoll(uint diceRoll)
        {
            return new Dictionary<Guid, ResourceCollection[]>();
        }
    }
}
