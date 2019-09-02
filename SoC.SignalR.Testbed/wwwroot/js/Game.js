"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

//Disable send button until connection is established
document.getElementById("joinGameRequest").disabled = true;

connection.on("GameListResponse", function (response) {
    var gamesList = document.getElementById("gamesList");
    var child = gamesList.firstElementChild;
    while (child) {
        e.removeChild(child);
        child = gamesList.firstElementChild;
    } 
    var li = document.createElement("p");
    if (response.gameInfo === null) {
        li.textContent = '(no games)';
    }
    gamesList.appendChild(li);
});

connection.start().then(function () {
    document.getElementById("joinGameRequest").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("joinGameRequest").addEventListener("click", function (event) {
    var request = {
        __typeName: 'JoinRequest',
        GameId: '{AC27329B-DB9A-4DB7-A976-287D6AD550B4}'
    }
    connection.invoke("PostRequest", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("getWaitingGamesRequest").addEventListener("click", function (event) {
    var request = {
    }
    connection.invoke("GetWaitingGamesRequest", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
