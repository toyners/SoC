"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

var gameId = null;

//Disable send button until connection is established
document.getElementById("joinGameRequest").disabled = true;

connection.on("GameListResponse", function (response) {
    var gamesList = document.getElementById("gamesList");
    var child = gamesList.firstElementChild;
    while (child) {
        gamesList.removeChild(child);
        child = gamesList.firstElementChild;
    } 
    if (response.gameInfo === null) {
        var row = document.createElement("tr");
        var cell = row.insertCell(0);
        cell.textContent = '(no games)';
        gamesList.appendChild(row);
    } else {
        var row = document.createElement("tr");
        var cell = row.insertCell(-1)
        cell.textContent = "NAME";

        cell = row.insertCell(-1);
        cell.textContent = "OWNER";

        cell = row.insertCell(-1);
        cell.textContent = "NO. OF SLOTS";

        cell = row.insertCell(-1);
        cell.textContent = "STATUS";

        gamesList.appendChild(row);

        response.gameInfo.forEach(function (gameInfo) {
            var row = document.createElement("tr");

            row.onclick = function () {
                return function () {
                    this.gameId = row.getAttribute('gameId');
                    row.classList.add('selected');
                    document.getElementById("joinGameRequest").disabled = false;
                };
            }(row);

            row.setAttribute('gameId', gameInfo.id);

            var cell = row.insertCell(-1);
            cell.textContent = gameInfo.name;

            cell = row.insertCell(-1);
            cell.textContent = gameInfo.owner;

            cell = row.insertCell(-1);
            cell.textContent = gameInfo.numberOfSlots + " / " + gameInfo.numberOfPlayers;

            cell = row.insertCell(-1);
            var gameStatus = "Open";
            cell.textContent = gameStatus;

            gamesList.appendChild(row);

            gameId = gameInfo.id;
        });
    }
});

connection.on("CreateGameResponse", function (response) {
    var status = document.getElementById("status");
    if (response.gameId === null) {
        status.textContent = "Game Creation Failed";
    }
});

connection.on("JoinGameResponse", function (response) {
    var status = document.getElementById("status");
    if (response.gameId === null) {
        status.textContent = "Game Join Failed";
    }
});

connection.start().then(function () {
    //document.getElementById("joinGameRequest").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("createGameRequest").addEventListener("click", function (event) {
    var userNameInput = document.getElementById("userName");
    var gameNameInput = document.getElementById("gameName");
    var request = {
        __typeName: 'CreateGameRequest',
        name: gameNameInput.value,
        username: userNameInput.value,
    }
    connection.invoke("CreateGame", request).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

document.getElementById("joinGameRequest").addEventListener("click", function (event) {
    var request = {
        GameId: gameId
    }
    connection.invoke("JoinGame", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("getWaitingGamesRequest").addEventListener("click", function (event) {
    var request = {}
    connection.invoke("GetWaitingGamesRequest", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

$("#gamesList tr").click(function () {
    this.gameId = $(this).getAttribute('gameId');
    document.getElementById("joinGameRequest").disabled = false;
});