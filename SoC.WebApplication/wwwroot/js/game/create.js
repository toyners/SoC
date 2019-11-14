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

var cellHeight = 90;
var cellWidth = 90;
var halfCellHeight = Math.trunc(cellHeight / 2);
var halfCellWidth = Math.trunc(cellWidth / 2);
var cellFragmentWidth = 68;

function displayBoard(state, layoutColumnData, hexData, textures) {
    var index = 0;
 
    for (var columnDataKey in layoutColumnData) {
        var columnData = layoutColumnData[columnDataKey];
        var y = columnData.y;
        var count = columnData.count;
        while (count-- > 0) {
            var hex = hexData[index++];
            var hexImage = new Kiwi.GameObjects.StaticImage(state, getResourceTexture(hex.resourceType, textures), columnData.x, y);
            state.addChild(hexImage);
            if (hex.productionFactor != 0) {
                var productionImage = new Kiwi.GameObjects.StaticImage(state, getProductionFactorTexture(hex.productionFactor, textures), columnData.x, y);
                state.addChild(productionImage);
            }
            y += cellHeight;
        }
    }
}

function getRoadTexture(index, textures) {
    switch (index) {
        case 0: return textures.roadhorizontalicon;
        case 1: return textures.roadhorizontaliconhover;
        case 2: return textures.four;
    }
}

function setupInitialRoadPlacementUI(state, roadPlacementData, textures, clickedHandler, hoverStartHandler, hoverEndHandler) {
    var result = new InitialRoadPlacementUI();
    for (var roadPlacementKey in roadPlacementData) {
        var placementData = roadPlacementData[roadPlacementKey];
        var x = placementData.x;
        var y = placementData.y;
        var roadImage = getRoadTexture(placementData.imageIndex, textures)
        var roadHoverImage = getRoadTexture(placementData.imageIndex + 1, textures)
        for (var index = 0; index < placementData.count; index++) {
            var roadIcon = new Kiwi.GameObjects.Sprite(state, roadImage, x, y);
            roadIcon.input.onUp.add(clickedHandler, state);
            roadIcon.input.onEntered.add(hoverStartHandler, state);
            roadIcon.visible = false;
            state.addChild(roadIcon);

            var roadHoverIcon = new Kiwi.GameObjects.Sprite(state, roadHoverImage, x, y);
            roadHoverIcon.visible = false;
            roadHoverIcon.input.onLeft.add(hoverEndHandler, state);
            state.addChild(roadHoverIcon);

            y += placementData.deltaY;
        }
    }

    return result;
}

function setupPlacementUI(state, layoutSettlementData, textures, clickedHandler, hoverStartHandler, hoverEndHandler) {
    var halfSettlementIconWidth = 11;
    var halfSettlementIconHeight = 11;
    var cellIndent = 20;
    var result = new SettlementPlacementUI();
    for (var settlementDataKey in layoutSettlementData) {
        var settlementData = layoutSettlementData[settlementDataKey];
        var x = settlementData.x - halfSettlementIconWidth;
        var y = settlementData.y - halfSettlementIconHeight;
        var total = (settlementData.count * 2) + 1;
        for (var count = 1; count <= total; count++) {
            var actualX = x + (count % 2 != 0 ? (settlementData.nudge * cellIndent) : 0);
            var settlementIcon = new Kiwi.GameObjects.Sprite(state, textures.settlementicon, actualX, y);
            settlementIcon.input.onUp.add(clickedHandler, state);
            settlementIcon.input.onEntered.add(hoverStartHandler, state);
            state.addChild(settlementIcon);

            var settlementHoverIcon = new Kiwi.GameObjects.Sprite(state, textures.redsettlementhover, actualX, y);
            settlementHoverIcon.input.onLeft.add(hoverEndHandler, state);
            settlementHoverIcon.visible = false;
            state.addChild(settlementHoverIcon);

            result.addSettlementPlacement(settlementIcon, settlementHoverIcon);

            y += halfCellHeight;
        }
    }

    return result;
}

function create() {
    Kiwi.State.prototype.create(this);
    this.background = new Kiwi.GameObjects.StaticImage(this, this.textures.background, 0, 0);
    var backgroundWidth = this.background.width;
    var backgroundHeight = this.background.height;
    this.addChild(this.background);

    var originX = (backgroundWidth / 2);
    var originY = (backgroundHeight / 2);
    displayBoard(this, getTilePlacementData(originX, originY), hexData, this.textures);

    this.initialRoadPlacementUI = setupInitialRoadPlacementUI(this, getRoadPlacementData(originX, originY), this.textures);

    this.settlementPlacementUI = setupPlacementUI(this, getSettlementPlacementData(originX, originY), this.textures, this.settlementIconClicked, this.settlementIconHoverStart, this.settlementIconHoverEnd)

    this.currentPlayerMarker = new Kiwi.GameObjects.Sprite(this, this.textures.playermarker, 90, 5);
    this.currentPlayerMarker.visible = false;
    this.currentPlayerMarker.animation.add('main', [2, 1, 0], 0.15, true, false);
    this.addChild(this.currentPlayerMarker);

    this.players = [];
    var player = new PlayerUI(playerNamesInOrder[0]);
    this.players.push(player);
    this.firstPlayerName = new Kiwi.GameObjects.Textfield(this, player.name, 10, 10, "#000", 32, 'normal', 'Impact');
    this.addChild(this.firstPlayerName);
    this.settlementCounter = new Kiwi.GameObjects.Textfield(this, '5x', 10, 50, "#000", 22, 'normal', 'Impact');
    this.addChild(this.settlementCounter);
    var settlementCounterIcon = new Kiwi.GameObjects.StaticImage(this, this.textures.redhouse, 35, 55);
    this.addChild(settlementCounterIcon);

    player = new PlayerUI(playerNamesInOrder[1]);
    this.players.push(player);
    this.secondPlayerName = new Kiwi.GameObjects.Textfield(this, player.name, 10, 550, "#000", 32, 'normal', 'Impact');
    this.settlementCounter = new Kiwi.GameObjects.Textfield(this, '5x', 10, 500, "#000", 22, 'normal', 'Impact');
    this.addChild(this.settlementCounter);
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