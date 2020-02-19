"use strict"

function processCollectedResources(gameState, resourcesCollectedByPlayerId) {
    for (var playerId in resourcesCollectedByPlayerId) {
        var player = gameState.playersById[playerId];
        var resourcesForPlayer = resourcesCollectedByPlayerId[playerId];
        if (resourcesForPlayer) {
            for (var resourcesForLocation of resourcesForPlayer) {
                var resources = resourcesForLocation.resources;
                if (resources.brickCount)
                    player.updateBrickCount(resources.brickCount);
                if (resources.grainCount)
                    player.updateGrainCount(resources.grainCount);
                if (resources.lumberCount)
                    player.updateLumberCount(resources.lumberCount);
                if (resources.oreCount)
                    player.updateOreCount(resources.oreCount);
                if (resources.woolCount)
                    player.updateWoolCount(resources.woolCount);
            }
        }
    }
}