"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var playerIdsByName = null;
var game = null;

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
        this.addImage('background', './images/background.png');
    };

    state.create = function () {
        Kiwi.State.prototype.create(this);
        this.background = new Kiwi.GameObjects.Sprite(this, this.textures.background, 0, 0);
        this.addChild(this.background);
    };

    var gameOptions = {
        width: 768,
        height: 512
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

    } else if (typeName === "PlayerTurnOrderCreator") {

    } else if (typeName === "PlaceSetupInfrastructureEvent") {

    }
}).catch(function (err) {
    return console.error(err.toString());
});
