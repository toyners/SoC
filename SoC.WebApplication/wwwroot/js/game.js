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
    };

    state.create = function () {
        Kiwi.State.prototype.create(this);
        this.background = new Kiwi.GameObjects.Sprite(this, this.textures.background, 0, 0);
        this.addChild(this.background);

        function getTexture(resourceType, textures) {
            if (!resourceType) {
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

        var index = Math.trunc(hexData.length / 2);
        var hex = hexData[index];
        this.hexSprites = [];

        this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getTexture(hex.resourceType, this.textures), 400 - 23, 300 - 23);
        this.addChild(this.hexSprites[index]);

        /*this.hex_one = new Kiwi.GameObjects.Sprite(this, this.textures.deserthex, 10, 54);
        this.addChild(this.hex_one);

        this.hex_two = new Kiwi.GameObjects.Sprite(this, this.textures.brickhex, 10, 99);
        this.addChild(this.hex_two);*/
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
