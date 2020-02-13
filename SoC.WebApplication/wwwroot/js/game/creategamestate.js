"use strict"

function createGameState() {
    Kiwi.State.prototype.create(this);
    this.background = new Kiwi.GameObjects.StaticImage(this, this.textures.background, 0, 0);
    var backgroundWidth = this.background.width;
    var backgroundHeight = this.background.height;
    this.addChild(this.background);

    var originX = (backgroundWidth / 2);
    var originY = (backgroundHeight / 2);
    displayBoard(this, getTilePlacementData(originX, originY), hexData, this.textures);

    this.players = setupPlayers(this, playerData)

    /*var player = new Player(this, playerNamesInOrder[0], 10, 10, true, true, "redSettlement");
    this.players.push(player);

    player = new Player(this, playerNamesInOrder[1], 10, 550, false, true);
    this.players.push(player);

    player = new Player(this, playerNamesInOrder[2], 700, 10, true, false);
    this.players.push(player);

    player = new Player(this, playerNamesInOrder[3], 700, 550, false, false);
    this.players.push(player);*/

    this.initialPlacementManager = setupInitialPlacementUI(this, this.textures,
        getSettlementPlacementData(originX, originY), getRoadPlacementData(originX, originY),
        this.imageIndexesById);

    this.currentPlayerMarker = new Kiwi.GameObjects.Sprite(this, this.textures.playermarker, 90, 5);
    this.currentPlayerMarker.visible = false;
    this.currentPlayerMarker.animation.add('main', [2, 1, 0], 0.15, true, false);
    this.addChild(this.currentPlayerMarker);

    
}