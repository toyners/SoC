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

    this.currentPlayer = null;
    this.currentPlayerIndex = -1;

    this.changeCurrentPlayer = function () {
        if (this.currentPlayerIndex === -1) {
            this.currentPlayerIndex = 0;
        } else if (this.currentPlayerIndex === this.players.length - 1) {
            this.currentPlayerIndex--;
        } else {
            this.currentPlayerIndex++;
        }

        var previousPlayer = this.currentPlayer || null;
        this.currentPlayer = this.players[this.currentPlayerIndex];

        return previousPlayer;
    };

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
        if (this.currentPlayer.isLocal) {
            var i = 1 / 0; // TODO: Should not get here
        }

        if (!this.unprocessedEvents.isEmpty()) {
            var gameEvent = this.unprocessedEvents.dequeue();
            if (gameEvent.playerId !== this.currentPlayer.id) {
                this.currentPlayer.deactivate();
                this.changeCurrentPlayer();
                this.currentPlayer.activate();

                if (!this.currentPlayer.isLocal) {
                    this.turnTimer.start();
                }
                return;
            }

            this.processEvent(gameEvent);

            var nextEvent = this.unprocessedEvents.peek();
            if (nextEvent) {
                if (nextEvent.playerId !== this.currentPlayer.id) {
                    this.currentPlayer.deactivate();
                    this.currentPlayer = this.changeCurrentPlayer();
                    this.currentPlayer.activate();
                    if (this.currentPlayer.isLocal) {
                        this.processEvent(this.unprocessedEvents.dequeue());
                    }
                    else {
                        this.turnTimer.start();
                    }
                } else {
                    this.pauseTimer.start();
                }
            }
        }
    }

    this.turnTimer = this.game.time.clock.createTimer('time1', 2, 3, false);
    this.turnTimer.createTimerEvent(Kiwi.Time.TimerEvent.TIMER_STOP, this.onTurnTimerStop, this);

    this.pauseTimer = this.game.time.clock.createTimer('time2', 1, 2, false);
    this.pauseTimer.createTimerEvent(Kiwi.Time.TimerEvent.TIMER_STOP, this.onTurnTimerStop, this);

    this.processEvent = function (gameEvent) {
        switch (gameEvent.typeName) {
            case "PlaceSetupInfrastructureEvent": {
                this.initialPlacementManager.activate();
                break;
            }
            case "SetupInfrastructurePlacedEvent": {
                this.initialPlacementManager.showPlacement(this.currentPlayer, gameEvent.settlementLocation, gameEvent.roadSegmentEndLocation);
                break;
            }
        }
    }
}