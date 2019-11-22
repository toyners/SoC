"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var playerNamesInOrder = null;
var playerIdsByName = null;
var playerNamesById = null;
var game = null;
var hexData = null;
var gameEvents = new Queue();

connection.start().then(function () {
    var fragments = window.location.pathname.split("/");
    var request = {
        gameId: fragments[2],
        playerId: fragments[3]
    };
    connection.invoke("ConfirmGameJoin", request).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function startGame() {
    var state = new Kiwi.State('Play');
    state.preload = preload;
 
    var backgroundWidth = 800;
    var backgroundHeight = 600;

    state.create = create;

    state.iconClicked = function (context, params) {
        if (context.hasTag('settlement')) {
            this.settlementPlacementUI.selectSettlement();
            this.initialRoadPlacementUI.showRoadSprites(context.id);
        } else {
            this.initialRoadPlacementUI.selectRoad();
        }
    };

    state.iconHoverStart = function (context, params) {
        if (context.hasTag('settlement'))
            this.settlementPlacementUI.toggleSettlementSprite(context.id);
        else
            this.initialRoadPlacementUI.toggle(context.id);
    }

    state.iconHoverEnd = function (context, params) {
        if (context.hasTag('settlement')) 
            this.settlementPlacementUI.toggleSettlementSprite(context.id);
        else
            this.initialRoadPlacementUI.toggle(context.id);
    }

    state.update = function () {
        Kiwi.State.prototype.update.call(this);

        if (!gameEvents.isEmpty()) {
            var gameEvent = gameEvents.dequeue();
            switch (gameEvent.typeName) {
                case "PlaceSetupInfrastructureEvent": {
                    this.currentPlayerMarker.visible = true;
                    this.currentPlayerMarker.animation.play('main');
                    break;
                }
                case "SetupInfrastructurePlacedEvent": {
                    // Placing infrastructure animation
                    break;
                }
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
        playerNamesById = {};
        for (var key in playerIdsByName) {
            var value = playerIdsByName[key];
            playerNamesById[value] = key;
        }
    } else if (typeName === "InitialBoardSetupEvent") {
        hexData = gameEvent.gameBoardSetup.hexData;
    } else if (typeName === "PlayerOrderEvent") {
        playerNamesInOrder = [];
        gameEvent.playerIds.forEach(function (playerId) {
            playerNamesInOrder.push(playerNamesById[playerId]);
        });
        startGame();
    } else {
        gameEvents.enqueue(gameEvent);
    }
}).catch(function (err) {
    return console.error(err.toString());
});