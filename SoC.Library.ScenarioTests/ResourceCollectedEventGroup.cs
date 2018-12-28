using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    internal class ResourceCollectedEventGroup
    {
        private readonly List<ResourceCollection> resourceCollectionList = new List<ResourceCollection>();
        private readonly LocalGameControllerScenarioRunner runner;
        internal Guid PlayerId { get; }

        internal ResourceCollectedEventGroup(Guid playerId, LocalGameControllerScenarioRunner runner)
        {
            this.PlayerId = playerId;
            this.runner = runner;
        }

        internal ResourceCollectedEventGroup AddResourceCollection(uint location, ResourceClutch resourceClutch)
        {
            this.resourceCollectionList.Add(new ResourceCollection(location, resourceClutch));
            return this;
        }

        internal LocalGameControllerScenarioRunner FinishResourcesCollectedEvent()
        {
            return this.runner.ResourcesCollectedEvent(this.PlayerId, this.resourceCollectionList.ToArray());
        }
    }
}
