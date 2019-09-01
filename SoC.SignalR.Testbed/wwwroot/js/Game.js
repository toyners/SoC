"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

//Disable send button until connection is established
document.getElementById("joinGameRequest").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
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
