"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var gameState = null;
var gameId = null;
var playerId = null;
//var playerNamesInOrder = null;
//var playerIdsByName = null;
//var playerNamesById = null;
//var imageIndexesById = null;
var game = null;
var hexData = null;
//var gameEvents = new Queue();

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
    //var gameState = new Kiwi.State('Play');
    gameState.preload = preloadGameState;

    gameState.imageIndexesById = imageIndexesById;
    gameState.create = createGameState;
    gameState.update = updateGameState;

    var gameOptions = {
        width: 800,
        height: 600
    };

    var game = new Kiwi.Game('game-container', 'soc', gameState, gameOptions);
}

connection.on("GameEvent", function (gameEvent) {
    var typeName = gameEvent.typeName;
    if (typeName === "GameJoinedEvent") {
        gameState = new Kiwi.State('Play');
        gameState.gameEvents = new Queue();
    } else if (typeName === "PlayerSetupEvent") {
        playerIdsByName = gameEvent.playerIdsByName;
        var settlementColourIndexes = [2, 4, 6, 8];
        var northEastRoadColourIndexes = [2, 4, 6, 8];
        var northWestRoadColourIndexes = [11, 13, 15, 17];
        var horizontalRoadColourIndexes = [2, 4, 6, 8];
        var playerData = {
            players: [],
            playerById: {}
        };

        playerNamesById = {};
        imageIndexesById = {};
        var index = 0;
        for (var playerName in playerIdsByName) {
            var playerId = playerIdsByName[playerName];

            //playerNamesById[playerId] = playerName;
            var imageIndexes = [
                settlementColourIndexes[index],
                northEastRoadColourIndexes[index],
                northWestRoadColourIndexes[index],
                horizontalRoadColourIndexes[index]
            ];
            //imageIndexesById[playerId] = imageIndexes;

            var player = {
                id: playerId,
                name: playerName,
                imageIndexes: imageIndexes
            }

            playerData.players.push(player);
            playerData.playerById[player.id] = player;

            index++;
        }

        gameState.playerData = playerData;

    } else if (typeName === "InitialBoardSetupEvent") {
        hexData = gameEvent.gameBoardSetup.hexData;
    } else if (typeName === "PlayerOrderEvent") {
        playerNamesInOrder = [];
        gameEvent.playerIds.forEach(function (playerId) {
            playerNamesInOrder.push(playerNamesById[playerId]);
        });
        main();
    } else {
        gameState.gameEvents.enqueue(gameEvent);
    }
}).catch(function (err) {
    return console.error(err.toString());
});
