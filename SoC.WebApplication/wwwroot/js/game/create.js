"use strict"

class PlayerUI {
    constructor(name) {
        this.name = name;
        this.settlementCount = 15;
        this.roadCount = 5;
        this.cityCount = 4;
    }

    addSettlement(texture, state) {
        var settlement = new Kiwi.GameObjects.Sprite(state, texture, x, y);
        state.addChild(settlement);
    }
}

function getResourceTexture(resourceType, textures) {
    if (resourceType == null) {
        return textures.deserthex;
    } else {
        switch (resourceType) {
            case 0: return textures.brickhex;
            case 1: return textures.grainhex;
            case 2: return textures.lumberhex;
            case 3: return textures.orehex;
            case 4: return textures.woolhex;
        }
    }
}

function getProductionFactorTexture(productionFactor, textures) {
    switch (productionFactor) {
        case 2: return textures.two;
        case 3: return textures.three;
        case 4: return textures.four;
        case 5: return textures.five;
        case 6: return textures.six;
        case 8: return textures.eight;
        case 9: return textures.nine;
        case 10: return textures.ten;
        case 11: return textures.eleven;
        case 12: return textures.twelve;
    }
}

function create() {
    Kiwi.State.prototype.create(this);
    this.background = new Kiwi.GameObjects.Sprite(this, this.textures.background, 0, 0);
    var backgroundWidth = this.background.width;
    var backgroundHeight = this.background.height;
    this.addChild(this.background);

    this.hexSprites = [];
    this.productionFactorSprites = [];

    var cellHeight = 45;
    var halfCellHeight = Math.trunc(45 / 2);
    var halfCellWidth = halfCellHeight;
    var cellFragmentWidth = 34;
    var startX = (backgroundWidth / 2) - halfCellWidth;
    var startY = (backgroundHeight / 2) - halfCellHeight;
    var x = startX;
    var y = startY;
    var index = 0;

    var layoutColumnData = [
        { x: startX - (2 * cellFragmentWidth), y: startY - cellHeight, count: 3 },
        { x: startX - cellFragmentWidth, y: startY - halfCellHeight - cellHeight, count: 4 },
        { x: startX, y: startY - (2 * cellHeight), count: 5 },
        { x: startX + cellFragmentWidth, y: startY - halfCellHeight - cellHeight, count: 4 },
        { x: startX + (2 * cellFragmentWidth), y: startY - cellHeight, count: 3 },
    ];

    for (var columnDataKey in layoutColumnData) {
        var columnData = layoutColumnData[columnDataKey];
        x = columnData.x;
        y = columnData.y;
        var count = columnData.count;
        while (count-- > 0) {
            var hex = hexData[index++];
            this.hexSprites[index] = new Kiwi.GameObjects.Sprite(this, getResourceTexture(hex.resourceType, this.textures), x, y);
            this.addChild(this.hexSprites[index]);
            if (hex.productionFactor != 0) {
                this.productionFactorSprites[index] = new Kiwi.GameObjects.Sprite(this, getProductionFactorTexture(hex.productionFactor, this.textures), x, y);
                this.addChild(this.productionFactorSprites[index]);
            }
            y += cellHeight;
        }
    }

    this.players = [];
    var player = new PlayerUI(playerNamesInOrder[0]);
    this.players.push(player);
    this.firstPlayerName = new Kiwi.GameObjects.Textfield(this, player.name, 10, 10, "#000", 32, 'normal', 'Impact');
    this.addChild(this.firstPlayerName);

    player = new PlayerUI(playerNamesInOrder[1]);
    this.players.push(player);
    this.secondPlayerName = new Kiwi.GameObjects.Textfield(this, player.name, 10, 550, "#000", 32, 'normal', 'Impact');
    this.addChild(this.secondPlayerName);

    player = new PlayerUI(playerNamesInOrder[2]);
    this.players.push(player);
    this.thirdPlayerName = new Kiwi.GameObjects.Textfield(this, player.name, 700, 10, "#000", 32, 'normal', 'Impact');
    this.addChild(this.thirdPlayerName);

    player = new PlayerUI(playerNamesInOrder[3]);
    this.players.push(player);
    this.fourthPlayerName = new Kiwi.GameObjects.Textfield(this, player.name, 700, 550, "#000", 32, 'normal', 'Impact');
    this.addChild(this.fourthPlayerName);
}