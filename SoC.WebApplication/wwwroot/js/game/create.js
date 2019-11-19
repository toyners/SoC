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

function displayBoard(state, layoutColumnData, hexData, textures) {
    var hexindex = 0;

    for (var index = 0; index < layoutColumnData.data.length; index++) {
        var columnData = layoutColumnData.data[index];
        var y = columnData.y;
        var count = columnData.count;
        while (count-- > 0) {
            var hex = hexData[hexindex++];
            var hexImage = new Kiwi.GameObjects.StaticImage(state, getResourceTexture(hex.resourceType, textures), columnData.x, y);
            state.addChild(hexImage);
            if (hex.productionFactor != 0) {
                var productionImage = new Kiwi.GameObjects.StaticImage(state, getProductionFactorTexture(hex.productionFactor, textures), columnData.x, y);
                state.addChild(productionImage);
            }
            y += layoutColumnData.deltaY;
        }
    }
}

function getRoadTexture(index, textures) {
    switch (index) {
        case 0: return textures.roadhorizontalicon;
        case 1: return textures.roadhorizontaliconhover;
        case 2: return textures.roadupperlefticon;
        case 3: return textures.roadupperlefticon;
    }
}

function setupPlacementUI(state, textures, settlementPlacementData, roadPlacementData, clickedHandler, hoverStartHandler, hoverEndHandler) {
    var halfSettlementIconWidth = 11;
    var halfSettlementIconHeight = 11;
    var cellIndent = 20;
    var settlementPlacementUI = new SettlementPlacementUI();
    var spriteIds = [];
    var icons = [];
    for (var index = 0; index < settlementPlacementData.data.length; index++) {
        var settlementData = settlementPlacementData.data[index];
        var x = settlementData.x - halfSettlementIconWidth;
        var y = settlementData.y - halfSettlementIconHeight;
        var total = (settlementData.count * 2) + 1;
        for (var count = 1; count <= total; count++) {
            var actualX = x + (count % 2 != 0 ? (settlementData.nudge * cellIndent) : 0);
            var settlementIcon = new Kiwi.GameObjects.Sprite(state, textures.settlementicon, actualX, y);
            settlementIcon.input.onUp.add(clickedHandler, state);
            settlementIcon.input.onEntered.add(hoverStartHandler, state);
            icons.push(settlementIcon);

            var settlementHoverIcon = new Kiwi.GameObjects.Sprite(state, textures.redsettlementhover, actualX, y);
            settlementHoverIcon.input.onLeft.add(hoverEndHandler, state);
            settlementHoverIcon.visible = false;
            icons.push(settlementHoverIcon);

            settlementPlacementUI.addSettlementPlacement(settlementIcon, settlementHoverIcon);
            spriteIds.push(settlementIcon.id);

            y += settlementPlacementData.deltaY;
        }
    }

    var roadPlacementUI = new InitialRoadPlacementUI();
    for (var index = 0; index < roadPlacementData.length; index++) {
        var roadData = roadPlacementData[index];
        var x = roadData.x;
        var y = roadData.y;
        var roadImage = getRoadTexture(roadData.imageIndex, textures)
        var roadHoverImage = getRoadTexture(roadData.imageIndex + 1, textures)
        var locations = roadData.locations;
        var locationIndex = 0;
        var count = roadData.count;
        while (count-- > 0) {
            var roadIcon = new Kiwi.GameObjects.Sprite(state, roadImage, x, y);
            //roadIcon.visible = false;
            //roadIcon.input.onUp.add(clickedHandler, state);
            //roadIcon.input.onEntered.add(hoverStartHandler, state);
            icons.push(roadIcon);

            roadPlacementUI.addRoadPlacement(spriteIds[locations[locationIndex++]], roadIcon);
            roadPlacementUI.addRoadPlacement(spriteIds[locations[locationIndex++]], roadIcon);

            var roadHoverIcon = new Kiwi.GameObjects.Sprite(state, roadHoverImage, x, y);
            roadHoverIcon.visible = false;
            //roadHoverIcon.input.onLeft.add(hoverEndHandler, state);
            icons.push(roadHoverIcon);

            y += roadData.deltaY;
        }
    }

    for (var i = icons.length - 1; i >= 0; i--)
        state.addChild(icons[i]);

    return [settlementPlacementUI, roadPlacementUI];
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

    var placementUIs = setupPlacementUI(this, this.textures,
        getSettlementPlacementData(originX, originY), getRoadPlacementData(originX, originY),
        this.settlementIconClicked, this.settlementIconHoverStart, this.settlementIconHoverEnd);
    this.settlementPlacementUI = placementUIs[0];
    this.initialRoadPlacementUI = placementUIs[1];

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