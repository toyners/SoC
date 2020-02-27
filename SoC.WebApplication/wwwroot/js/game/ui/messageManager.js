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
        if (this.messageLines.length <= this.messageLineFields.length) {
            for (var messageLineField of this.messageLineFields) {
                if (messageLineField.text == "") {
                    messageLineField.text = text;
                    break;
                }
            }
            // Should not get here
        } else {
            var index = 1;
            for (; index < this.messageLineFields.length; index++) {
                this.messageLineFields[index - 1].text = this.messageLineFields[index].text;
            }

            this.messageLineFields[index - 1].text = text;
        }
    }
}
