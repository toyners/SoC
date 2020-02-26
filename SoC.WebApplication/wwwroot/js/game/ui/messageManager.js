"use strict"

class MessageManager {
    constructor(gameState, layout) {
        this.messageLineFields = [];
        this.messageLines = [];

        for (var coords of layout) {
            var messageLineField = new Kiwi.GameObjects.Textfield(gameState, "", coords.x, coords.y, "#000", 16, 'normal', 'impact');
            this.messageLineFields.push(messageLineField);
            gameState.addChild(messageLineField);
        }
    }

    addLine(text) {
        this.messageLines.push(text);
        for (var messageLineField of this.messageLineFields) {
            if (messageLineField.text == "") {
                messageLineField.text = text;
                return;
            }
        }

        for ()
    }
}
