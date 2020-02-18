
class Player {
    constructor(gameState, playerData, x, y, topDown, leftRight) {
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;
        this.brickCount = 0;
        this.grainCount = 0;
        this.lumberCount = 0;
        this.oreCount = 0;
        this.woolCount = 0;

        var diffY = topDown ? 40 : -40;
        var diffX = leftRight ? 65 : -65;

        var playerName = new Kiwi.GameObjects.Textfield(gameState, playerData.name, x, y, "#000", 32, 'normal', 'Impact');
        gameState.addChild(playerName);

        y += diffY;

        var settlementIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.settlementIcons, x, y);
        settlementIcon.cellIndex = playerData.imageIndexes[0];
        gameState.addChild(settlementIcon);

        this.settlementCounter = new Kiwi.GameObjects.Textfield(gameState, this.settlementCount, x + 25, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.settlementCounter);

        this.cityCounter = new Kiwi.GameObjects.Textfield(gameState, this.cityCount, x + diffX, y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.cityCounter);

        var roadIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.roadIcons, x + (2 * diffX) + 28, y);
        roadIcon.cellIndex = playerData.imageIndexes[3];
        gameState.addChild(roadIcon);

        this.roadCounter = new Kiwi.GameObjects.Textfield(gameState, this.roadCount, x + (2 * diffX), y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.roadCounter);

        y += diffY;
        var brickIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x, y);
        brickIcon.cellIndex = 0;
        gameState.addChild(brickIcon);

        this.brickCounter = new Kiwi.GameObjects.Textfield(gameState, this.brickCount, x + 20, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.brickCounter);

        var grainIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x + 50, y);
        grainIcon.cellIndex = 1;
        gameState.addChild(grainIcon);

        this.grainCounter = new Kiwi.GameObjects.Textfield(gameState, this.grainCount, x + 70, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.grainCounter);

        var lumberIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x + 100, y);
        lumberIcon.cellIndex = 2;
        gameState.addChild(lumberIcon);

        this.lumberCounter = new Kiwi.GameObjects.Textfield(gameState, this.lumberCount, x + 120, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.lumberCounter);

        var oreIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x + 150, y);
        oreIcon.cellIndex = 3;
        gameState.addChild(oreIcon);

        this.oreCounter = new Kiwi.GameObjects.Textfield(gameState, this.oreCount, x + 170, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.oreCounter);

        var woolIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, x + 200, y);
        woolIcon.cellIndex = 4;
        gameState.addChild(woolIcon);

        this.woolCounter = new Kiwi.GameObjects.Textfield(gameState, this.woolCount, x + 220, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.woolCounter);
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