"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var playerIdsByName = null;
var game = null;
var hexData = null;

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
    state.preload = function () {
        Kiwi.State.prototype.preload(this);
        this.addImage('background', '../../../images/background.png');
        this.addImage('brickhex', '../../../images/hextypes/brick.png');
        this.addImage('deserthex', '../../../images/hextypes/desert.png');
        this.addImage('grainhex', '../../../images/hextypes/grain.png');
        this.addImage('lumberhex', '../../../images/hextypes/lumber.png');
        this.addImage('orehex', '../../../images/hextypes/ore.png');
        this.addImage('woolhex', '../../../images/hextypes/wool.png');
        this.addImage('two', '../../../images/productionfactors/2.png');
        this.addImage('three', '../../../images/productionfactors/3.png');
        this.addImage('four', '../../../images/productionfactors/4.png');
        this.addImage('five', '../../../images/productionfactors/5.png');
        this.addImage('six', '../../../images/productionfactors/6.png');
        this.addImage('eight', '../../../images/productionfactors/8.png');
        this.addImage('nine', '../../../images/productionfactors/9.png');
        this.addImage('ten', '../../../images/productionfactors/10.png');
        this.addImage('eleven', '../../../images/productionfactors/11.png');
        this.addImage('twelve', '../../../images/productionfactors/12.png');
    };

    state.create = function () {
        Kiwi.State.prototype.create(this);
        this.background = new Kiwi.GameObjects.Sprite(this, this.textures.background, 0, 0);
        this.addChild(this.background);

        var layoutColumnData = [
            { x: 10, y: 54, count: 3 },
            { x: 44, y: 32, count: 4 },
            { x: 78, y: 10, count: 5 },
            { x: 112, y: 32, count: 4 },
            { x: 146, y: 54, count: 3 },
        ];

        var cellHeight = 45;

        this.hexSprites = [];
        this.productionFactorSprites = [];

        var startingIndex = Math.trunc(hexData.length / 2);
        var startX = 400 - 23;
        var startY = 300 - 23;
        var x = startX;
        var y = startY;
        var index = startingIndex;

        var hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        x = startX - 34;
        y = startY - 22 + (2 * cellHeight);

        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        x = startX - (2 * 34);
        y = startY + cellHeight;

        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y - cellHeight;
        index = index - 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        x = startX;
        y = startY + cellHeight;
        index = startingIndex + 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y + cellHeight;
        index = index + 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        x = startX + 34;
        y = startY + 22 - (2 * cellHeight);

        index = index + 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y + cellHeight;
        index = index + 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y + cellHeight;
        index = index + 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }

        y = y + cellHeight;
        index = index + 1;
        hex = hexData[index];
        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
        this.addChild(this.hexSprites[index]);
        if (hex.productionFactor != 0) {
            this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
            this.addChild(this.productionFactorSprites[index]);
        }
    };

    var gameOptions = {
        width: 800,
        height: 600
    };

    game = new Kiwi.Game('game-container', 'soc', state, gameOptions);
}

connection.on("GameEvent", function (response) {
    var typeName = response.typeName;
    if (typeName === "GameJoinedEvent") {
        startGame();
    } else if (typeName === "PlayerSetupEvent") {
        playerIdsByName = response.playerIdsByName;
    } else if (typeName === "InitialBoardSetupEvent") {
        hexData = response.gameBoardSetup.hexData;
    } else if (typeName === "PlayerTurnOrderCreator") {

    } else if (typeName === "PlaceSetupInfrastructureEvent") {

    }
}).catch(function (err) {
    return console.error(err.toString());
});
