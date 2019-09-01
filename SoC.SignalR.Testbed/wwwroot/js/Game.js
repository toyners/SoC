"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameRequest").build();

//Disable send button until connection is established
document.getElementById("sendRequest").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendRequest").disabled = false;
}).catch(function (err) {
        return console.error(err.toString());
});

document.getElementById("sendRequest").addEventListener("click", function (event) {
    var request = {
        __typeName: 'Request'
    }
    connection.invoke("PostRequest", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});