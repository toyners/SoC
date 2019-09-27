"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

$(window).on("load", function () {
    var gameId, playerId
    var request = {
        gameId: gameId,
        playerId: playerId
    };
    connection.invoke("ConfirmGameJoin", request).catch(function (err) {
        return console.error(err.toString());
    });
});