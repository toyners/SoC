"use strict"

class MessageManager {
    constructor(gameState, layout) {
        this.messageTextFieldsByPlayerId = {};

        for (var playerData of gameState.playerData.players) {
            var coords = layout.shift();
            var messageField = new Kiwi.GameObjects.Textfield(gameState, "", coords.x, coords.y, "#000", 16, 'normal', 'impact');
            messageField.visible = false;
            this.messageTextFieldsByPlayerId[playerData.id] = messageField;
            gameState.addChild(messageField);
        }
        
        //this.timer = gameState.game.time.clock.createTimer('time', 1, 5, false);
        //this.timer.createTimerEvent(Kiwi.Time.TimerEvent.TIMER_STOP, gameState.onTimerStop, gameState);
    }

    hideText(playerId) {
        this.messageTextFieldsByPlayerId[playerId].visible = false;
    }

    showText(playerId, text) {
        this.messageTextFieldsByPlayerId[playerId].text = text;
        this.messageTextFieldsByPlayerId[playerId].visible = true;
    }
}