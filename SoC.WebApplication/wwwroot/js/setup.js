"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameSetup").build();

var gameId = null;

//Disable send button until connection is established
document.getElementById("joinGameRequest").disabled = true;

connection.on("GameSessionList", function (response) {
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

connection.on("GameSessionCreated", function (response) {
    var status = document.getElementById("status");
    if (response.gameId === null) {
        status.textContent = "Game Creation Failed";
    }
});

connection.on("GameLaunched", function (response) {
    var url = window.location.href + 'Game/' + response.gameId;
    window.location.replace(url)
});

connection.start().then(function () {
    createGame();
}).catch(function (err) {
    return console.error(err.toString());
});

function createGame() {
    // TODO: Validation of inputs
    var userNameInput = document.getElementById("userName");
    var gameNameInput = document.getElementById("gameName");

    var playerCountInput = document.getElementById("playerCount");
    var playerCount = parseInt(playerCountInput.value);

    var botCountInput = document.getElementById("botCount");
    var botCount = parseInt(botCountInput.value);

    var turnTimeoutInput = document.getElementById("turnTimeout");
    var turnTimeout = parseInt(turnTimeoutInput.value);

    var playerStartsInput = document.getElementById("playerStarts");
    var playerStarts = playerStartsInput.value === "on";

    var request = {
        name: gameNameInput.value,
        username: userNameInput.value,
        maxplayers: playerCount,
        maxbots: botCount,
        turntimeoutinseconds: turnTimeout,
        playerstarts: playerStarts
    }
    connection.invoke("CreateGameSession", request).catch(function (err) {
        return console.error(err.toString());
    });
}

Mousetrap.bind('ctrl+c', function (e) {
    createGame();
});

document.getElementById("createGameRequest").addEventListener("click", function (event) {
    createGame();
    event.preventDefault();
});

document.getElementById("joinGameRequest").addEventListener("click", function (event) {
    var userNameInput = document.getElementById("userName");
    var request = {
        GameId: gameId,
        username: userNameInput.value,
    }
    connection.invoke("JoinGameSession", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("GameSessionJoined", function (response) {
    var status = document.getElementById("status");
    if (response.gameId === null) {
        status.textContent = "Game Join Failed";
    }

    if (response.className === "GameJoinedResponse") {

    } else if (response.className === "LaunchGameResponse") {
        var url = window.location.href + 'Game/' + response.gameId + '/' + response.playerId;
        window.location.replace(url)
    }
});

document.getElementById("getWaitingGamesRequest").addEventListener("click", function (event) {
    var request = {}
    connection.invoke("GetWaitingGameSessions", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

$("#gamesList tr").click(function () {
    this.gameId = $(this).getAttribute('gameId');
    document.getElementById("joinGameRequest").disabled = false;
});

