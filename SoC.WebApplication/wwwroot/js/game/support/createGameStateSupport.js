"use strict"

function displayBoard(gameState, layoutColumnData, hexData, textures) {
    var hexindex = 0;

    for (var index = 0; index < layoutColumnData.data.length; index++) {
        var columnData = layoutColumnData.data[index];
        var y = columnData.y;
        var count = columnData.count;
        while (count-- > 0) {
            var hex = hexData[hexindex++];
            var hexImage = new Kiwi.GameObjects.StaticImage(gameState, textures.hextypes, columnData.x, y);
            hexImage.cellIndex = hex.resourceType != null ? hex.resourceType : 5;
            gameState.addChild(hexImage);
            if (hex.productionFactor != 0) {
                var productionImage = new Kiwi.GameObjects.StaticImage(gameState, textures.productionfactors, columnData.x, y);
                productionImage.cellIndex = hex.productionFactor;
                gameState.addChild(productionImage);
            }
            y += layoutColumnData.deltaY;
        }
    }
}

function setupPlayers(gameState, playerData) {
    var players = [];

    players.push(new Player(gameState, playerData[0], 10, 10, true, true));
    players.push(new Player(gameState, playerData[1], 10, 550, false, true));
    players.push(new Player(gameState, playerData[2], 700, 10, true, false));
    players.push(new Player(gameState, playerData[3], 700, 550, false, false));

    return players;
}

function setupInitialPlacementUI(gameState, textures, settlementPlacementData, roadPlacementData, imageIndexesById) {
    var initialPlacementManager = new InitialPlacementManager(gameState, textures, imageIndexesById,
        function (context, params) { initialPlacementManager.onConfirm(); },
        function (context, params) { initialPlacementManager.onCancelSettlement(); },
        function (context, params) { initialPlacementManager.onCancelRoad(); });

    var sprites = [];

    var settlementClickedHandler = function (context, params) {
        initialPlacementManager.handleSettlementClick(context.id);
    };

    var settlementHoverEnterHandler = function (context, params) {
        initialPlacementManager.handleSettlementEnter(context.id);
    }

    var settlementHoverLeftHandler = function (context, params) {
        initialPlacementManager.handleSettlementLeft(context.id);
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
            var settlementSprite = new Kiwi.GameObjects.Sprite(gameState, textures.settlement, x, y);
            settlementSprite.visible = false;
            settlementSprite.input.onUp.add(settlementClickedHandler, gameState);
            settlementSprite.input.onEntered.add(settlementHoverEnterHandler, gameState);
            settlementSprite.input.onLeft.add(settlementHoverLeftHandler, gameState);

            initialPlacementManager.addSettlementSprite(settlementSprite, settlementLocation++);
            sprites.push(settlementSprite);

            if (offsetCount > 1) {
                x += offsets[offsetIndex].deltaX;
                y += offsets[offsetIndex++].deltaY;
            }

            offsetCount--;
        }
    }

    var roadClickedHandler = function (context, params) {
        initialPlacementManager.handleRoadClick(context.id);
    }

    var roadHoverEnterHandler = function (context, params) {
        initialPlacementManager.handleRoadHoverEnter(context.id);
    }

    var roadHoverLeftHandler = function (context, params) {
        initialPlacementManager.handleRoadHoverLeft(context.id);
    }

    for (var roadCollectionData of roadPlacementData) {
        for (var roadData of roadCollectionData.roads) {
            var roadSprite = new Kiwi.GameObjects.Sprite(gameState, textures[roadCollectionData.imageName], roadData.x, roadData.y);
            roadSprite.cellIndex = roadCollectionData.imageIndex;
            roadSprite.visible = false;
            roadSprite.input.onUp.add(roadClickedHandler, gameState);
            roadSprite.input.onEntered.add(roadHoverEnterHandler, gameState);
            roadSprite.input.onLeft.add(roadHoverLeftHandler, gameState);

            var locations = roadData.locations;
            initialPlacementManager.addRoadPlacement(roadSprite, roadCollectionData.imageIndex, roadCollectionData.hoverImageIndex,
                roadCollectionData.type, sprites[locations[0]].id, locations[0], sprites[locations[1]].id, locations[1]);
            sprites.push(roadSprite);
        }
    }

    for (var i = sprites.length - 1; i >= 0; i--)
        gameState.addChild(sprites[i]);

    return initialPlacementManager;
}