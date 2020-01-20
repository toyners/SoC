"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var gameId = null;
var playerId = null;
var playerNamesInOrder = null;
var playerIdsByName = null;
var playerNamesById = null;
var imageIndexesById = null;
var game = null;
var hexData = null;
var gameEvents = new Queue();
var initialPlacements = 0;

connection.start().then(function () {
    var fragments = window.location.pathname.split("/");
    gameId = fragments[2];
    playerId = fragments[3];
    var request = {
        gameId: gameId,
        playerId: playerId
    };
    connection.invoke("ConfirmGameJoin", request).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function main() {
    var state = new Kiwi.State('Play');
    state.preload = preload;

    var backgroundWidth = 800;
    var backgroundHeight = 600;

    state.imageIndexesById = imageIndexesById;
    state.create = create;

    state.update = function () {
        Kiwi.State.prototype.update.call(this);

        if (!gameEvents.isEmpty()) {
            var gameEvent = gameEvents.dequeue();
            switch (gameEvent.typeName) {
                case "PlaceSetupInfrastructureEvent": {
                    initialPlacements += 1;
                    this.initialPlacementUI.showSettlementSprites();
                    this.currentPlayerMarker.visible = true;
                    this.currentPlayerMarker.animation.play('main');
                    break;
                }
                case "SetupInfrastructurePlacedEvent": {
                    this.initialPlacementUI.addPlacement(gameEvent.playerId, gameEvent.settlementLocation, gameEvent.roadSegmentEndLocation);
                    // Placing infrastructure animation

                    break;
                }
            }
        }

        if (this.initialPlacementUI && this.initialPlacementUI.isConfirmed()) {
            this.currentPlayerMarker.visible = false;
            var placementData = this.initialPlacementUI.getData();
            if (placementData) {
                var request = {
                    gameId: gameId,
                    playerId: playerId,
                    playerActionType: 'PlaceSetupInfrastructureAction',
                    data: JSON.stringify({
                        initiatingPlayerId: playerId,
                        settlementLocation: placementData.settlementLocation,
                        roadEndLocation: placementData.roadEndLocation
                    })
                };

                if (initialPlacements == 2)
                    this.initialPlacementUI = null;
                else
                    this.initialPlacementUI.reset();

                connection.invoke("PlayerAction", request).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }
    };

    var gameOptions = {
        width: backgroundWidth,
        height: backgroundHeight
    };

    game = new Kiwi.Game('game-container', 'soc', state, gameOptions);
}

connection.on("GameEvent", function (gameEvent) {
    var typeName = gameEvent.typeName;
    if (typeName === "GameJoinedEvent") {

    } else if (typeName === "PlayerSetupEvent") {
        playerIdsByName = gameEvent.playerIdsByName;
        var settlementColourIndexes = [2, 4, 6, 8];
        var northEastRoadColourIndexes = [2, 4, 6, 8];
        var northWestRoadColourIndexes = [11, 13, 15, 17];
        var horizontalRoadColourIndexes = [2, 4, 6, 8];
        playerNamesById = {};
        imageIndexesById = {};
        var index = 0;
        for (var playerName in playerIdsByName) {
            var playerId = playerIdsByName[playerName];
            playerNamesById[playerId] = playerName;
            var imageIndexes = [
                settlementColourIndexes[index],
                northEastRoadColourIndexes[index],
                northWestRoadColourIndexes[index],
                horizontalRoadColourIndexes[index]
            ];
            imageIndexesById[playerId] = imageIndexes;
            index++;
        }
    } else if (typeName === "InitialBoardSetupEvent") {
        hexData = gameEvent.gameBoardSetup.hexData;
    } else if (typeName === "PlayerOrderEvent") {
        playerNamesInOrder = [];
        gameEvent.playerIds.forEach(function (playerId) {
            playerNamesInOrder.push(playerNamesById[playerId]);
        });
        main();
    } else {
        gameEvents.enqueue(gameEvent);
    }
}).catch(function (err) {
    return console.error(err.toString());
});
