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

function sendRequest(playerAction, connection) {
    var request = {
        gameId: playerAction.gameId,
        playerId: playerAction.id,
        playerActionType: playerAction.type,
        data: JSON.stringify(playerAction.data)
    };

    connection.invoke("PlayerAction", request).catch(function (err) {
        return console.error(err.toString());
    });
}