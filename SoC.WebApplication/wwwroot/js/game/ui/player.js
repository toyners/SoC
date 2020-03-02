
class Player {
    constructor(gameState, playerData, initData) {
        this.id = playerData.id;
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;
        this.brickCount = 0;
        this.grainCount = 0;
        this.lumberCount = 0;
        this.oreCount = 0;
        this.woolCount = 0;
        this.resourceCount = 0;
        this.isLocal = playerData.isLocal;

        this.currentPlayerMarker = new Kiwi.GameObjects.Sprite(gameState, initData.marker.image, initData.marker.x, initData.marker.y);
        this.currentPlayerMarker.visible = false;
        this.currentPlayerMarker.animation.add('main', [2, 1, 0], 0.15, true, false);
        gameState.addChild(this.currentPlayerMarker);

        var layout = initData.layout;
        var line = layout.shift();

        var playerName = new Kiwi.GameObjects.Textfield(gameState, playerData.name, line.x, line.y, "#000", 32, 'normal', 'Impact');
        gameState.addChild(playerName);

        line = layout.shift();

        var settlementIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.settlementIcons, line.x, line.y);
        settlementIcon.cellIndex = playerData.imageIndexes[4];
        gameState.addChild(settlementIcon);

        this.settlementCounter = new Kiwi.GameObjects.Textfield(gameState, this.settlementCount, line.x + 25, line.y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.settlementCounter);

        var roadIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.roadIcons, line.x + 55, line.y);
        roadIcon.cellIndex = playerData.imageIndexes[4];
        gameState.addChild(roadIcon);

        this.roadCounter = new Kiwi.GameObjects.Textfield(gameState, this.roadCount, line.x + 80, line.y, "#000", 22, 'normal', 'Impact');
        gameState.addChild(this.roadCounter);

        //this.cityCounter = new Kiwi.GameObjects.Textfield(gameState, this.cityCount, line.x + 65, line.y, "#000", 22, 'normal', 'Impact');
        //gameState.addChild(this.cityCounter);

        if (this.isLocal) {
            line = layout.shift();
            var brickIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x, line.y);
            brickIcon.cellIndex = 0;
            gameState.addChild(brickIcon);

            this.brickCounter = new Kiwi.GameObjects.Textfield(gameState, this.brickCount, line.x + 25, line.y, "#000", 20, 'normal', 'Impact');
            gameState.addChild(this.brickCounter);

            var grainIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 55, line.y);
            grainIcon.cellIndex = 1;
            gameState.addChild(grainIcon);

            this.grainCounter = new Kiwi.GameObjects.Textfield(gameState, this.grainCount, line.x + 80, line.y, "#000", 20, 'normal', 'Impact');
            gameState.addChild(this.grainCounter);

            var lumberIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 110, line.y);
            lumberIcon.cellIndex = 2;
            gameState.addChild(lumberIcon);

            this.lumberCounter = new Kiwi.GameObjects.Textfield(gameState, this.lumberCount, line.x + 135, line.y, "#000", 20, 'normal', 'Impact');
            gameState.addChild(this.lumberCounter);

            var oreIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 165, line.y);
            oreIcon.cellIndex = 3;
            gameState.addChild(oreIcon);

            this.oreCounter = new Kiwi.GameObjects.Textfield(gameState, this.oreCount, line.x + 190, line.y, "#000", 20, 'normal', 'Impact');
            gameState.addChild(this.oreCounter);

            var woolIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x + 220, line.y);
            woolIcon.cellIndex = 4;
            gameState.addChild(woolIcon);

            this.woolCounter = new Kiwi.GameObjects.Textfield(gameState, this.woolCount, line.x + 245, line.y, "#000", 20, 'normal', 'Impact');
            gameState.addChild(this.woolCounter);
        } else {
            line = layout.shift();

            var resourceIcon = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.resourceTypes, line.x, line.y);
            resourceIcon.cellIndex = 5;
            gameState.addChild(resourceIcon);

            this.resourceCounter = new Kiwi.GameObjects.Textfield(gameState, this.resourceCount, line.x + 25, line.y, "#000", 20, 'normal', 'Impact');
            gameState.addChild(this.resourceCounter);
        }
    }

    activate() {
        this.currentPlayerMarker.visible = true;
        this.currentPlayerMarker.animation.play('main');
    }

    deactivate() {
        this.currentPlayerMarker.visible = false;
        this.currentPlayerMarker.animation.play('main');
    }

    decrementRoadCount() {
        this.roadCount--;
        this.roadCounter.text = this.roadCount;
    }

    decrementSettlementCount() {
        this.settlementCount--;
        this.settlementCounter.text = this.settlementCount;
    }

    incrementSettlementCount() {
        this.settlementCount++;
        this.settlementCounter.text = this.settlementCount;
    }

    updateBrickCount(delta) {
        if (this.isLocal) {
            this.brickCount += delta;
            this.brickCounter.text = this.brickCount;
        } else {
            this.resourceCount += delta
            this.resourceCounter.text = this.resourceCount;
        }
    }

    updateGrainCount(delta) {
        if (this.isLocal) {
            this.grainCount += delta;
            this.grainCounter.text = this.grainCount;
        } else {
            this.resourceCount += delta
            this.resourceCounter.text = this.resourceCount;
        }
    }

    updateLumberCount(delta) {
        if (this.isLocal) {
            this.lumberCount += delta;
            this.lumberCounter.text = this.lumberCount;
        } else {
            this.resourceCount += delta
            this.resourceCounter.text = this.resourceCount;
        }
    }

    updateOreCount(delta) {
        if (this.isLocal) {
            this.oreCount += delta;
            this.oreCounter.text = this.oreCount;
        } else {
            this.resourceCount += delta
            this.resourceCounter.text = this.resourceCount;
        }
    }

    updateWoolCount(delta) {
        if (this.isLocal) {
            this.woolCount += delta;
            this.woolCounter.text = this.woolCount;
        } else {
            this.resourceCount += delta
            this.resourceCounter.text = this.resourceCount;
        }
    }
}