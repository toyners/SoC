"use strict"

class MessageManager {
    constructor(gameState, layout) {
        this.messageLines = [];

        for (var coords of layout) {
            var messageField = new Kiwi.GameObjects.Textfield(gameState, "", coords.x, coords.y, "#000", 16, 'normal', 'impact');
            this.messageLines.push(messageField);
            gameState.addChild(messageField);
        }
    }

    addLine(text) {
        for (var messageLine of this.messageLines) {
            if (messageLine.text == "") {
                messageLine.text = text;
                break;
            }
        }
    }
}
