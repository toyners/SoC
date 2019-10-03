"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

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

    }
}).catch(function (err) {
    return console.error(err.toString());
});

/*$(window).on("load", function () {
    var gameId, playerId
    var request = {
        gameId: gameId,
        playerId: playerId
    };
    connection.invoke("ConfirmGameJoin", request).catch(function (err) {
        return console.error(err.toString());
    });
});*/