
class Player {
    constructor(gameState, name, x, y, topDown, leftRight, settlementIconName) {
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;

        var diffY = topDown ? 40 : -40;
        var diffX = leftRight ? 55 : -55;

        var playerName = new Kiwi.GameObjects.Textfield(gameState, name, x, y, "#000", 32, 'normal', 'Impact');
        gameState.addChild(playerName);

        y += diffY;

        this.settlementCounter = new Kiwi.GameObjects.Textfield(gameState, this.settlementCount + 'x', x, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.settlementCounter);

        if (settlementIconName) {
            var settlementIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures[settlementIconName], x + 25, y - 5);
            gameState.addChild(settlementIcon);
        }

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