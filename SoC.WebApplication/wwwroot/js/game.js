"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var playerIdsByName = null;

var state = new Kiwi.State('Play');
state.preload = function () {
    var i = 0;
}

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

connection.on("GameEvent", function (response) {
    var typeName = response.typeName;
    if (typeName === "GameJoinedEvent") {

    } else if (typeName === "PlayerSetupEvent") {
        playerIdsByName = response.playerIdsByName;
    } else if (typeName === "InitialBoardSetupEvent") {

    } else if (typeName === "PlayerTurnOrderCreator") {

    } else if (typeName === "PlaceSetupInfrastructureEvent") {

    }
}).catch(function (err) {
    return console.error(err.toString());
});