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
            var hexImage = new Kiwi.GameObjects.StaticImage(state, textures.hextypes, columnData.x, y);
            hexImage.cellIndex = hex.resourceType != null ? hex.resourceType : 5;
            state.addChild(hexImage);
            if (hex.productionFactor != 0) {
                var productionImage = new Kiwi.GameObjects.StaticImage(state, textures.productionfactors, columnData.x, y);
                productionImage.cellIndex = hex.productionFactor;
                state.addChild(productionImage);
            }
            y += layoutColumnData.deltaY;
        }
    }
}

function setupInitialPlacementUI(state, textures, settlementPlacementData, roadPlacementData, settlementImageIndexById) {
    var initialPlacementUI = new InitialPlacementUI(state, textures, 1, settlementImageIndexById,
        function (context, params) { initialPlacementUI.onConfirm(); },
        function (context, params) { initialPlacementUI.onCancelSettlement(); },
        function (context, params) { initialPlacementUI.onCancelRoad(); });

    var sprites = [];

    var settlementClickedHandler = function (context, params) {
        initialPlacementUI.selectSettlement();
        initialPlacementUI.showRoadSprites(context.id);
    };

    var settlementHoverHandler = function (context, params) {
        initialPlacementUI.toggleSettlementSprite(context.id);
    }

    var halfSettlementIconWidth = 12;
    var halfSettlementIconHeight = 12;
    var settlementLocation = 0;
    for (var index = 0; index < settlementPlacementData.length; index++) {
        var columnPlacementData = settlementPlacementData[index];
        var x = columnPlacementData.x - halfSettlementIconWidth;
        var y = columnPlacementData.y - halfSettlementIconHeight;
        var offsets = columnPlacementData.offsets;
        var offsetCount = offsets.length + 1;
        var offsetIndex = 0;
        while (offsetCount > 0) {
            var settlementSprite = new Kiwi.GameObjects.Sprite(state, textures.settlement, x, y);
            settlementSprite.visible = false;
            settlementSprite.input.onUp.add(settlementClickedHandler, state);
            settlementSprite.input.onEntered.add(settlementHoverHandler, state);
            settlementSprite.input.onLeft.add(settlementHoverHandler, state);

            initialPlacementUI.addSettlementSprite(settlementSprite, settlementLocation++);
            sprites.push(settlementSprite);

            if (offsetCount > 1) {
                x += offsets[offsetIndex].deltaX;
                y += offsets[offsetIndex++].deltaY;
            }

            offsetCount--;
        }
    }

    var roadClickedHandler = function (context, params) {
        initialPlacementUI.selectRoad();
    }

    var roadHoverHandler = function (context, params) {
        initialPlacementUI.toggleRoadSprite(context.id);
    }

    for (var roadCollectionData of roadPlacementData) {
        for (var roadData of roadCollectionData.roads) {
            var roadSprite = new Kiwi.GameObjects.Sprite(state, textures[roadCollectionData.imageName], roadData.x, roadData.y);
            roadSprite.cellIndex = roadCollectionData.imageIndex;
            //roadSprite.visible = false;
            roadSprite.input.onUp.add(roadClickedHandler, state);
            roadSprite.input.onEntered.add(roadHoverHandler, state);
            roadSprite.input.onLeft.add(roadHoverHandler, state);

            var locations = roadData.locations;
            initialPlacementUI.addRoadForSettlement(roadSprite, sprites[locations[0]].id, locations[0], locations[1]);
            initialPlacementUI.addRoadForSettlement(roadSprite, sprites[locations[1]].id, locations[1], locations[0]);
            initialPlacementUI.addRoadPlacement(roadSprite);
            sprites.push(roadSprite);
        }
    }

    /*for (var index = 0; index < roadPlacementData.length; index++) {
        var roadData = roadPlacementData[index];
        var roadSprite = new Kiwi.GameObjects.Sprite(state, textures['roads'], roadData.x, roadData.y);
        roadSprite.cellIndex = roadData.imageIndex;
        //roadSprite.visible = false;
        roadSprite.input.onUp.add(roadClickedHandler, state);
        roadSprite.input.onEntered.add(roadHoverHandler, state);
        roadSprite.input.onLeft.add(roadHoverHandler, state);

        var locations = roadData.locations;
        initialPlacementUI.addRoadForSettlement(roadSprite, sprites[locations[0]].id, locations[0], locations[1]);
        initialPlacementUI.addRoadForSettlement(roadSprite, sprites[locations[1]].id, locations[1], locations[0]);
        initialPlacementUI.addRoadPlacement(roadSprite);
        sprites.push(roadSprite);
    }*/

    for (var i = sprites.length - 1; i >= 0; i--)
        state.addChild(sprites[i]);

    return initialPlacementUI;
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

    this.initialPlacementUI = setupInitialPlacementUI(this, this.textures,
        getSettlementPlacementData(originX, originY), getRoadPlacementData(originX, originY),
        this.settlementImageIndexById);

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