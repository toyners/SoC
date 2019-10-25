"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var playerNamesInOrder = null;
var playerIdsByName = null;
var playerNamesById = null;
var game = null;
var hexData = null;
var gameEvents = new Queue();

connection.start().then(function () {
    //document.getElementById("joinGameRequest").disabled = false;
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

function getResourceTexture(resourceType, textures) {
    if (resourceType == null) {
        return textures.deserthex;
    } else {
        switch (resourceType) {
            case 0: return textures.brickhex;
            case 1: return textures.grainhex;
            case 2: return textures.lumberhex;
            case 3: return textures.orehex;
            case 4: return textures.woolhex;
        }
    }
}

function getProductionFactorTexture(productionFactor, textures) {
    switch (productionFactor) {
        case 2: return textures.two;
        case 3: return textures.three;
        case 4: return textures.four;
        case 5: return textures.five;
        case 6: return textures.six;
        case 8: return textures.eight;
        case 9: return textures.nine;
        case 10: return textures.ten;
        case 11: return textures.eleven;
        case 12: return textures.twelve;
    }
}

function startGame() {
    var state = new Kiwi.State('Play');
    state.preload = preload;
 
    var backgroundWidth = 800;
    var backgroundHeight = 600;

    state.create = function () {
        Kiwi.State.prototype.create(this);
        this.background = new Kiwi.GameObjects.Sprite(this, this.textures.background, 0, 0);
        this.addChild(this.background);

        this.hexSprites = [];
        this.productionFactorSprites = [];

        var cellHeight = 45;
        var halfCellHeight = Math.trunc(45 / 2);
        var halfCellWidth = halfCellHeight;
        var cellFragmentWidth = 34;
        var startX = (backgroundWidth / 2) - halfCellWidth;
        var startY = (backgroundHeight / 2) - halfCellHeight;
        var x = startX;
        var y = startY;
        var index = 0;

        var layoutColumnData = [
            { x: startX - (2 * cellFragmentWidth), y: startY - cellHeight, count: 3 },
            { x: startX - cellFragmentWidth, y: startY - halfCellHeight - cellHeight, count: 4 },
            { x: startX, y: startY - (2 * cellHeight), count: 5 },
            { x: startX + cellFragmentWidth, y: startY - halfCellHeight - cellHeight, count: 4 },
            { x: startX + (2 * cellFragmentWidth), y: startY - cellHeight, count: 3 },
        ];

        for (var columnDataKey in layoutColumnData) {
            var columnData = layoutColumnData[columnDataKey];
            x = columnData.x;
            y = columnData.y;
            var count = columnData.count;
            while (count-- > 0) {
                var hex = hexData[index++];
                this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
                this.addChild(this.hexSprites[index]);
                if (hex.productionFactor != 0) {
                    this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
                    this.addChild(this.productionFactorSprites[index]);
                }
                y += cellHeight;
            }
        }

        this.firstPlayerName = new Kiwi.GameObjects.Textfield(this, playerNamesInOrder[0], 10, 10, "#000", 32, 'normal', 'Impact');
        this.addChild(this.firstPlayerName);

        this.secondPlayerName = new Kiwi.GameObjects.Textfield(this, playerNamesInOrder[1], 10, 550, "#000", 32, 'normal', 'Impact');
        this.addChild(this.secondPlayerName);

        this.thirdPlayerName = new Kiwi.GameObjects.Textfield(this, playerNamesInOrder[2], 700, 10, "#000", 32, 'normal', 'Impact');
        this.addChild(this.thirdPlayerName);

        this.fourthPlayerName = new Kiwi.GameObjects.Textfield(this, playerNamesInOrder[3], 700, 550, "#000", 32, 'normal', 'Impact');
        this.addChild(this.fourthPlayerName);
    };

    state.update = function () {
        Kiwi.State.prototype.update.call(this);

        if (!gameEvents.isEmpty()) {
            var gameEvent = gameEvents.dequeue();
            if (gameEvent.typeName === "SetupInfrastructurePlacedEvent") {
                // Placing infrastructure animation
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
