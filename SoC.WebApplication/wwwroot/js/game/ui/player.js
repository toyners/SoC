
class Player {
    constructor(gameState, playerData, x, y, topDown, leftRight) {
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;
        this.brickCount = 0;
        this.grainCount = 0;

        var diffY = topDown ? 40 : -40;
        var diffX = leftRight ? 65 : -65;

        var playerName = new Kiwi.GameObjects.Textfield(gameState, playerData.name, x, y, "#000", 32, 'normal', 'Impact');
        gameState.addChild(playerName);

        y += diffY;

        this.settlementCounter = new Kiwi.GameObjects.Textfield(gameState, this.settlementCount + 'x', x, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.settlementCounter);

        var settlementIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.settlement, x + 25, y - 3);
        settlementIcon.cellIndex = playerData.imageIndexes[0];
        settlementIcon.scaleX = 0.8;
        settlementIcon.scaleY = 0.8;
        gameState.addChild(settlementIcon);

        this.cityCounter = new Kiwi.GameObjects.Textfield(gameState, this.cityCount + 'x', x + diffX, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.cityCounter);

        this.roadCounter = new Kiwi.GameObjects.Textfield(gameState, this.roadCount + 'x', x + (2 * diffX), y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.roadCounter);

        var roadIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.horizontalRoads, x + (2 * diffX) + 28, y + 5);
        roadIcon.cellIndex = playerData.imageIndexes[3];
        roadIcon.scaleX = 0.6;
        roadIcon.scaleY = 0.8;
        gameState.addChild(roadIcon);

        y += diffY;
        var brickIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x, y);
        brickIcon.cellIndex = 0;
        gameState.addChild(brickIcon);

        this.brickCounter = new Kiwi.GameObjects.Textfield(gameState, ':' + this.brickCount, x + 20, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.brickCounter);

        var grainIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x + 50, y);
        grainIcon.cellIndex = 1;
        gameState.addChild(grainIcon);

        this.grainCounter = new Kiwi.GameObjects.Textfield(gameState, ':' + this.grainCount, x + 70, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.grainCounter);
    }

    decrementRoadCount() {
        this.roadCount--;
        this.roadCounter.text = this.roadCount + 'x';
    }

    decrementSettlementCount() {
        this.settlementCount--;
        this.settlementCounter.text = this.settlementCount + 'x'
    }

    incrementSettlementCount() {
        this.settlementCount++;
        this.settlementCounter.text = this.settlementCount + 'x'
    }
}