"use strict"

function createGameState() {
    Kiwi.State.prototype.create(this);
    this.background = new Kiwi.GameObjects.StaticImage(this, this.textures.background, 0, 0);
    var backgroundWidth = this.background.width;
    var backgroundHeight = this.background.height;
    this.addChild(this.background);

    var originX = (backgroundWidth / 2);
    var originY = (backgroundHeight / 2);
    displayBoard(this, getTilePlacementData(originX, originY), this.textures);

    this.playersById = setupPlayers(this)

    this.initialPlacementManager = setupInitialPlacementUI(this, this.textures,
        getSettlementPlacementData(originX, originY), getRoadPlacementData(originX, originY));

    this.currentPlayerMarker = new Kiwi.GameObjects.Sprite(this, this.textures.playermarker, 90, 5);
    this.currentPlayerMarker.visible = false;
    this.currentPlayerMarker.animation.add('main', [2, 1, 0], 0.15, true, false);
    this.addChild(this.currentPlayerMarker);

    this.diceOne = new Kiwi.GameObjects.Sprite(this, this.textures.dice, 50, (backgroundHeight / 2) - 50);
    this.diceOne.visible = false;
    this.addChild(this.diceOne);

    this.diceTwo = new Kiwi.GameObjects.Sprite(this, this.textures.dice, 100, (backgroundHeight / 2) - 50);
    this.diceTwo.visible = false;
    this.addChild(this.diceTwo);

    this.end = new Kiwi.GameObjects.Sprite(this, this.textures.end, 10, (backgroundHeight / 2) - 90);
    this.end.visible = false;
    this.addChild(this.end);

    var buttonToggleHandler = function (context, params) {
        context.cellIndex = context.cellIndex == 0 ? 1 : 0;
    };

    //this.end.input.onUp.add(settlementClickedHandler, gameState);
    this.end.input.onEntered.add(buttonToggleHandler, gameState);
    this.end.input.onLeft.add(buttonToggleHandler, gameState);

}