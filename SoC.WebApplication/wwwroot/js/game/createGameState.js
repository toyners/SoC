"use strict"

var BUTTON_NORMAL = 0;
var BUTTON_HIGHLIGHTED = 1;
var BUTTON_DISABLED = 2;

function createGameState() {
    Kiwi.State.prototype.create(this);

    this.unprocessedEvents = new Queue();

    this.buttonToggleHandler = function (context, params) {
        if (context.visible && context.cellIndex !== BUTTON_DISABLED) {
            context.cellIndex = context.cellIndex == BUTTON_NORMAL ? BUTTON_HIGHLIGHTED : BUTTON_NORMAL;
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

    this.buildHandler = function (context) {
        if (context.visible && context.cellIndex !== BUTTON_DISABLED) {
            context.visible = false;

            var gameState = context.parent;
            gameState.hideTurnMenu();
            gameState.showBuildMenu();
        }
    }

    this.build = createButton(this, 'build', 10, (backgroundHeight / 2) - 90, this.buildHandler, this.buttonToggleHandler);
    this.addChild(this.build);

    this.back = new Kiwi.GameObjects.Sprite(this, this.textures.back, 10, (backgroundHeight / 2) - 90);
    this.back.visible = false;
    this.addChild(this.back);

    this.backHandler = function (context) {
        if (context.visible) {
            context.visible = false;

            var gameState = context.parent;
            gameState.hideBuildMenu();
            gameState.showTurnMenu();
        }
    }

    this.back.input.onUp.add(this.backHandler, gameState);
    this.back.input.onEntered.add(this.buttonToggleHandler, gameState);
    this.back.input.onLeft.add(this.buttonToggleHandler, gameState);

    this.showTurnMenu = function () {
        this.build.visible = true;
    }

    this.hideTurnMenu = function () {
        this.build.visible = false;
    }

    this.showBuildMenu = function () {
        this.back.visible = true;
    }

    this.hideBuildMenu = function () {
        this.back.visible = false;
    }

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