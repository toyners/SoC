"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

//Disable send button until connection is established
document.getElementById("joinGameRequest").disabled = true;

connection.on("GameListResponse", function (response) {
    var gamesList = document.getElementById("gamesList");
    var child = gamesList.firstElementChild;
    while (child) {
        gamesList.removeChild(child);
        child = gamesList.firstElementChild;
    } 
    var li = document.createElement("p");
    if (response.gameInfo === null) {
        li.textContent = '(no games)';
        gamesList.appendChild(li);
    } else {
        response.gameInfo.forEach(function (gameInfo) {
            var li = document.createElement("li");
            li.textContent = gameInfo.name + ' ' ;
            gamesList.appendChild(li);
        });
    }
});

connection.on("CreateGameResponse", function (response) {
    var status = document.getElementById("status");
    if (response.gameId !== null) {
        status.textContent = "Game Created";
    } else {
        status.textContent = "Game Creation Failed";
    }
});

connection.start().then(function () {
    document.getElementById("joinGameRequest").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("createGameRequest").addEventListener("click", function (event) {
    var gameName = document.getElementById("gameName");
    var request = {
        __typeName: 'CreateGameRequest',
        name: gameName.value
    }
    connection.invoke("CreateGame", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
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
