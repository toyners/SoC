
class Player {
    constructor(gameState, playerData, x, y, topDown, leftRight) {
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;

        var diffY = topDown ? 40 : -40;
        var diffX = leftRight ? 55 : -55;

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

        x += diffX;
        this.cityCounter = new Kiwi.GameObjects.Textfield(gameState, this.cityCount + 'x', x, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.cityCounter);

        x += diffX;
        this.roadCounter = new Kiwi.GameObjects.Textfield(gameState, this.roadCount + 'x', x, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.roadCounter);
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