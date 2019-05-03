
namespace SoC.Library.ScenarioTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Jabberwocky.SoC.Library;

    internal class CollectedResourcesBuilder
    {
        private Dictionary<string, List<ResourceCollection>> resourceCollectionsByPlayerName = new Dictionary<string, List<ResourceCollection>>();

        public CollectedResourcesBuilder Add(string playerName, uint location, ResourceClutch resources)
        {
            if (!this.resourceCollectionsByPlayerName.TryGetValue(playerName, out var resourcesCollection))
            {
                resourcesCollection = new List<ResourceCollection>();   
                this.resourceCollectionsByPlayerName.Add(playerName, resourcesCollection);
            }

            resourcesCollection.Add(new ResourceCollection(location, resources));

            return this;
        }

        public Dictionary<string, ResourceCollection[]> Build()
        {
            var result = new Dictionary<string, ResourceCollection[]>();

            foreach(var kv in this.resourceCollectionsByPlayerName)
            {
                var resourceCollection = kv.Value.OrderBy(k => k.Location).ToArray();
                result.Add(kv.Key, resourceCollection);
            }

            return result;
        }
    }
}
