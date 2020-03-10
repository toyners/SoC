﻿"use strict"

function displayBoard(gameState, layoutColumnData) {
    var hexData = gameState.hexData;
    var hexindex = 0;

    for (var index = 0; index < layoutColumnData.data.length; index++) {
        var columnData = layoutColumnData.data[index];
        var y = columnData.y;
        var count = columnData.count;
        while (count-- > 0) {
            var hex = hexData[hexindex++];
            var hexImage = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.hextypes, columnData.x, y);
            hexImage.cellIndex = hex.resourceType != null ? hex.resourceType : 5;
            gameState.addChild(hexImage);
            if (hex.productionFactor != 0) {
                var productionImage = new Kiwi.GameObjects.StaticImage(gameState, gameState.textures.productionfactors, columnData.x, y);
                productionImage.cellIndex = hex.productionFactor;
                gameState.addChild(productionImage);
            }
            y += layoutColumnData.deltaY;
        }
    }
}

function setupMessageManagers(gameState) {
    var messageManagersByPlayerId = {};

    messageManagersByPlayerId[gameState.playerData.players[0].id] = new MessageManager(gameState,
        [{ x: 10, y: 120 }, { x: 10, y: 140 }, { x: 10, y: 160 }, { x: 10, y: 180 }]);
    messageManagersByPlayerId[gameState.playerData.players[1].id] = new MessageManager(gameState,
        [{ x: 10, y: 460 }, { x: 10, y: 440 }, { x: 10, y: 420 }, { x: 10, y: 400 }]);
    messageManagersByPlayerId[gameState.playerData.players[2].id] = new MessageManager(gameState,
        [{ x: 600, y: 120 }, { x: 600, y: 140 }, { x: 600, y: 160 }, { x: 600, y: 180 }]);
    messageManagersByPlayerId[gameState.playerData.players[3].id] = new MessageManager(gameState,
        [{ x: 600, y: 460 }, { x: 600, y: 440 }, { x: 600, y: 420 }, { x: 600, y: 400 }]);
    return messageManagersByPlayerId;
}

function setupPlayers(gameState) {
    var playersById = {};
    var players = [];

    var player = new Player(gameState, gameState.playerData.players[0],
    {
        layout: [{ x: 10, y: 10 }, { x: 10, y: 50 }, { x: 10, y: 80 }],
        marker: { image: gameState.textures.marker, x: 100, y: 5 }
    });
    playersById[gameState.playerData.players[0].id] = player;
    players.push(player);


    player = new Player(gameState, gameState.playerData.players[1],
    {
        layout: [{ x: 10, y: 550 }, { x: 10, y: 520 }, { x: 10, y: 480 }],
        marker: { image: gameState.textures.marker, x: 100, y: 545 }
    });
    playersById[gameState.playerData.players[1].id] = player;
    players.push(player);

    player = new Player(gameState, gameState.playerData.players[2],
    {
        layout: [{ x: 700, y: 10 }, { x: 700, y: 50 }, { x: 700, y: 80 }],
        marker: { image: gameState.textures.reverseMarker, x: 640, y: 5 }
    });
    playersById[gameState.playerData.players[2].id] = player;
    players.push(player);
    
    player = new Player(gameState, gameState.playerData.players[3],
    {
        layout: [{ x: 700, y: 550 }, { x: 700, y: 520 }, { x: 700, y: 480 }],
        marker: { image: gameState.textures.reverseMarker, x: 640, y: 545 }
    });
    playersById[gameState.playerData.players[3].id] = player;
    players.push(player);

    gameState.playersById = playersById;
    gameState.players = players;
}

function setupInitialPlacementUI(gameState, settlementPlacementData, roadPlacementData) {
    var initialPlacementManager = new InitialPlacementManager(gameState);

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
            var settlementSprite = new Kiwi.GameObjects.Sprite(gameState, gameState.textures.settlement, x, y);
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
            var roadSprite = new Kiwi.GameObjects.Sprite(gameState, gameState.textures[roadCollectionData.imageName], roadData.x, roadData.y);
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