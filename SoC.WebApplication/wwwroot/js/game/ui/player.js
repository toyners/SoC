
class Player {
    constructor(gameState, playerData, layout) {
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;
        this.brickCount = 0;
        this.grainCount = 0;
        this.lumberCount = 0;
        this.oreCount = 0;
        this.woolCount = 0;

        var line = layout.shift();

        var playerName = new Kiwi.GameObjects.Textfield(gameState, playerData.name, line.x, line.y, "#000", 32, 'normal', 'Impact');
        gameState.addChild(playerName);

        line = layout.shift();

        var settlementIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.settlementIcons, line.x, line.y);
        settlementIcon.cellIndex = playerData.imageIndexes[4];
        gameState.addChild(settlementIcon);

        this.settlementCounter = new Kiwi.GameObjects.Textfield(gameState, this.settlementCount, line.x + 25, line.y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.settlementCounter);

        this.cityCounter = new Kiwi.GameObjects.Textfield(gameState, this.cityCount, line.x + 65,line.y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.cityCounter);

        var roadIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.roadIcons, line.x + 160, line.y);
        roadIcon.cellIndex = playerData.imageIndexes[4];
        gameState.addChild(roadIcon);

        this.roadCounter = new Kiwi.GameObjects.Textfield(gameState, this.roadCount, line.x + 130, line.y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.roadCounter);

        line = layout.shift();
        var brickIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x, line.y);
        brickIcon.cellIndex = 0;
        gameState.addChild(brickIcon);

        this.brickCounter = new Kiwi.GameObjects.Textfield(gameState, this.brickCount, line.x + 25, line.y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.brickCounter);

        var grainIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 50, line.y);
        grainIcon.cellIndex = 1;
        gameState.addChild(grainIcon);

        this.grainCounter = new Kiwi.GameObjects.Textfield(gameState, this.grainCount, line.x + 70, line.y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.grainCounter);

        var lumberIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 100, line.y);
        lumberIcon.cellIndex = 2;
        gameState.addChild(lumberIcon);

        this.lumberCounter = new Kiwi.GameObjects.Textfield(gameState, this.lumberCount, line.x + 120, line.y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.lumberCounter);

        var oreIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 150, line.y);
        oreIcon.cellIndex = 3;
        gameState.addChild(oreIcon);

        this.oreCounter = new Kiwi.GameObjects.Textfield(gameState, this.oreCount, line.x + 170, line.y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.oreCounter);

        var woolIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 200, line.y);
        woolIcon.cellIndex = 4;
        gameState.addChild(woolIcon);

        this.woolCounter = new Kiwi.GameObjects.Textfield(gameState, this.woolCount, line.x + 220, line.y, "#000", 20, 'normal', 'Impact');
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