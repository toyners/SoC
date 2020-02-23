"use strict"

class MessageManager {
    constructor(gameState) {
        this.messageText = new Kiwi.GameObjects.Textfield(gameState, "", 10, 50, "#000", 16, 'impact');
        this.messageText.visible = false;
        gameState.addChild(this.messageText);

        gameState.onTimerStop = function () {
            this.messageManager.hideText();
        }

        this.timer = gameState.game.time.clock.createTimer('time', 2, 5, false);
        this.timer.createTimerEvent(Kiwi.Time.TimerEvent.TIMER_STOP, gameState.onTimerStop, gameState);
    }


    hideText() {
        this.messageText.visible = false;
    }

    setText(text) {
        this.messageText.text = text;
        this.messageText.visible = true;

        if (!this.timer.isStopped())
            this.timer.stop();

        this.timer.start();

        // You can call the createTimer method on any clock to attach a timer to the clock.
	    /**
	    * Param 1 - Name of Timer.
	    * Param 2 - Delay Between Counts.
	    * Param 3 - Repeat amount. If set to -1 will repeat infinitely.
	    * Param 4 - If the timer should start.
	    */
        //gameState.timer = gameState.game.time.clock.createTimer('time', 2, 5, true);
        //gameState.timer.createTimerEvent(Kiwi.Time.TimerEvent.TIMER_STOP, this.onTimerStop, gameState);
    }
}