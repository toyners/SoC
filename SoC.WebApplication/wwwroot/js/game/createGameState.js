"use strict"

var BUTTON_NORMAL = 0;
var BUTTON_HIGHLIGHTED = 1;
var BUTTON_DISABLED = 2;

var BUTTON_ID_UNSET = 0;
var BUTTON_BACK_ID = 1;
var BUTTON_BUILD_ID = 2;
var BUTTON_SETTLEMENT_ID = 3;

function createGameState() {
    Kiwi.State.prototype.create(this);

    this.unprocessedEvents = new Queue();

    this.onDownPressButtonId = 0;

    this.buttonEnterHandler = function (context) {
        if (context.visible && context.cellIndex !== BUTTON_DISABLED) {
            context.cellIndex = BUTTON_HIGHLIGHTED;
        }
    };

    this.buttonLeftHandler = function (context) {
        if (context.visible && context.cellIndex !== BUTTON_DISABLED) {
            context.cellIndex = BUTTON_NORMAL;
        }
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

    this.diceOne = new Kiwi.GameObjects.Sprite(this, this.textures.dice, (backgroundWidth / 2) - 50, backgroundHeight - 50);
    this.diceOne.visible = false;
    this.addChild(this.diceOne);

    this.diceTwo = new Kiwi.GameObjects.Sprite(this, this.textures.dice, (backgroundWidth / 2) + 10, backgroundHeight - 50);
    this.diceTwo.visible = false;
    this.addChild(this.diceTwo);

    this.showTurnMenu = function () {
        this.build.visible = true;
    }

    this.hideTurnMenu = function () {
        this.build.visible = false;
    }

    this.showBuildMenu = function () {
        this.back.visible = true;
        this.back.cellIndex = BUTTON_HIGHLIGHTED;
        this.settlement.visible = true;

    }

    this.hideBuildMenu = function () {
        this.back.visible = false;
    }

    this.onUpBuildHandler = function (context) {
        if (context.visible && context.cellIndex !== BUTTON_DISABLED) {
            var gameState = context.parent;
            if (gameState.onDownPressButtonId === BUTTON_BUILD_ID) {
                gameState.hideTurnMenu();
                gameState.showBuildMenu();
                gameState.onDownPressButtonId = BUTTON_ID_UNSET;
            }
        }
    }

    this.onDownBuildHandler = function (context) {
        if (context.visible && context.cellIndex !== BUTTON_DISABLED) {
            context.parent.onDownPressButtonId = BUTTON_BUILD_ID;
        }
    }

    this.build = createButton(this, 'build', 10, (backgroundHeight / 2) - 90,
        this.onDownBuildHandler, this.onUpBuildHandler, this.buttonEnterHandler, this.buttonLeftHandler);
    this.addChild(this.build);

    this.onUpBackHandler = function (context) {
        if (context.visible) {
            var gameState = context.parent;
            if (gameState.onDownPressButtonId === BUTTON_BACK_ID) {
                gameState.hideBuildMenu();
                gameState.showTurnMenu();
                gameState.onDownPressButtonId = BUTTON_ID_UNSET;
            }
        }
    }

    this.onDownBackHandler = function (context) {
        if (context.visible) {
            context.parent.onDownPressButtonId = BUTTON_BACK_ID;
        }
    }

    var onDownHandler = function (buttonId) {
        return new function (context) {
            if (context.visible) {
                context.parent.onDownPressButtonId = buttonId;
            }
        };
    }

    this.back = createButton(this, 'back', 10, (backgroundHeight / 2) - 90,
        this.onDownBackHandler, this.onUpBackHandler, this.buttonEnterHandler, this.buttonLeftHandler);
    this.addChild(this.back);

    this.settlement = createButton(this, 'settlement', 10, (backgroundHeight / 2) - 50,
        new function (context) { }, new function (context) { }, this.buttonEnterHandler, this.buttonLeftHandler);
    this.addChild(this.settlement);

    this.currentPlayer = null;
    this.playerSetupOrder = this.players.concat(this.players.slice(0, 3).reverse());

    this.changeCurrentPlayer = function () {

        var previousPlayer = this.currentPlayer || null;
        this.currentPlayer = this.playerSetupOrder.shift();

        return previousPlayer;
    };

    this.end = new Kiwi.GameObjects.Sprite(this, this.textures.end, 10, (backgroundHeight / 2) + 30);
    this.end.visible = false;
    this.addChild(this.end);

    this.endTurnHandler = function (context) {
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
    this.end.input.onEntered.add(this.buttonEnterHandler, gameState);
    this.end.input.onLeft.add(this.buttonLeftHandler, gameState);

    this.onTurnTimerStop = function () {
        if (this.currentPlayer.isLocal) {
            var i = 1 / 0; // TODO: Should not get here
        }

        if (!this.unprocessedEvents.isEmpty()) {
            var gameEvent = this.unprocessedEvents.dequeue();
            this.processEvent(gameEvent);

            var nextEvent = this.unprocessedEvents.peek();
            if (nextEvent) {
                if (nextEvent.playerId !== this.currentPlayer.id) {
                    this.currentPlayer.deactivate();
                    this.changeCurrentPlayer();
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

    this.turnTimer = this.game.time.clock.createTimer('time1', 1, 2, false);
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