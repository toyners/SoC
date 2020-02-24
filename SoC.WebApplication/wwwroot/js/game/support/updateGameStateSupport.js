"use strict"

function processCollectedResources(gameState, resourcesCollectedByPlayerId) {
    for (var playerId in resourcesCollectedByPlayerId) {
        var player = gameState.playersById[playerId];
        var resourcesForPlayer = resourcesCollectedByPlayerId[playerId];
        if (resourcesForPlayer) {
            for (var resourcesForLocation of resourcesForPlayer) {
                var message = "";
                var resources = resourcesForLocation.resources;
                if (resources.brickCount) {
                    player.updateBrickCount(resources.brickCount);
                    message += resources.brickCount + ' brick, ';
                }
                if (resources.grainCount) {
                    player.updateGrainCount(resources.grainCount);
                    message += resources.grainCount + ' grain, ';
                }
                if (resources.lumberCount) {
                    player.updateLumberCount(resources.lumberCount);
                    message += resources.lumberCount + ' lumber, ';
                }
                if (resources.oreCount) {
                    player.updateOreCount(resources.oreCount);
                    message += resources.oreCount + ' ore, ';
                }
                if (resources.woolCount) {
                    player.updateWoolCount(resources.woolCount);
                    message += resources.woolCount + ' wool, ';
                }
            }

            if (message == "")
                message = "No resources collected";
            else
                message = "Collected " + message;
            gameState.messageManager.showText(playerId, message);
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