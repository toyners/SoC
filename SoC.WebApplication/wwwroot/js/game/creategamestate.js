"use strict"

function createGameState() {
    Kiwi.State.prototype.create(this);

    this.unprocessedEvents = new Queue();

    this.buttonToggleHandler = function (context, params) {
        context.cellIndex = context.cellIndex == 0 ? 1 : 0;
    };

    this.background = new Kiwi.GameObjects.StaticImage(this, this.textures.background, 0, 0);
    var backgroundWidth = this.background.width;
    var backgroundHeight = this.background.height;
    this.addChild(this.background);

    var originX = (backgroundWidth / 2);
    var originY = (backgroundHeight / 2);
    displayBoard(this, getTilePlacementData(originX, originY));

    setupPlayers(this)

    this.messageManagersByPlayerId = setupMessageManagers(this);

    this.initialPlacementManager = setupInitialPlacementUI(this,
        getSettlementPlacementData(originX, originY), getRoadPlacementData(originX, originY));

    this.diceOne = new Kiwi.GameObjects.Sprite(this, this.textures.dice, 50, (backgroundHeight / 2) - 50);
    this.diceOne.visible = false;
    this.addChild(this.diceOne);

    this.diceTwo = new Kiwi.GameObjects.Sprite(this, this.textures.dice, 100, (backgroundHeight / 2) - 50);
    this.diceTwo.visible = false;
    this.addChild(this.diceTwo);

    this.end = new Kiwi.GameObjects.Sprite(this, this.textures.end, 10, (backgroundHeight / 2) - 90);
    this.end.visible = false;
    this.addChild(this.end);

    this.endTurnHandler = function (context, params) {
        if (context.visible) {
            this.playerActions.enqueue({
                id: this.playerId,
                gameId: this.gameId,
                type: 'EndOfTurnAction',
                data: { initiatingPlayerId: this.playerId }
            });

            context.visible = false;
        }
    }

    this.end.input.onUp.add(this.endTurnHandler, gameState);
    this.end.input.onEntered.add(this.buttonToggleHandler, gameState);
    this.end.input.onLeft.add(this.buttonToggleHandler, gameState);

    this.onTurnTimerStop = function () {
        var index = 0;
        index++;
    }

    this.turnTimer = this.game.time.clock.createTimer('time', 2, 3, false);
    this.turnTimer.createTimerEvent(Kiwi.Time.TimerEvent.TIMER_STOP, this.onTurnTimerStop, this);
}