"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

$(window).on("load", function () {
    connection.invoke("ConfirmGameJoin").catch(function (err) {
        return console.error(err.toString());
    });
});