"use strict"

var tileHeight = 90;
var tileWidth = 90;
var halfTileHeight = Math.trunc(tileHeight / 2);
var halfTileWidth = Math.trunc(tileWidth / 2);
var threeQuarterTileWidth = 68;

function getTilePlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    return {
        deltaY: tileHeight,
        data: [
            { x: startX - (2 * threeQuarterTileWidth), y: startY - tileHeight, count: 3 },
            { x: startX - threeQuarterTileWidth, y: startY - halfTileHeight - tileHeight, count: 4 },
            { x: startX, y: startY - (2 * tileHeight), count: 5 },
            { x: startX + threeQuarterTileWidth, y: startY - halfTileHeight - tileHeight, count: 4 },
            { x: startX + (2 * threeQuarterTileWidth), y: startY - tileHeight, count: 3 }
        ]
    };
}

function getSettlementPlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    return {
        deltaY: halfTileHeight,
        data: [
            { x: startX - (2 * threeQuarterTileWidth), y: startY - tileHeight, count: 3, nudge: 1 },
            { x: startX - threeQuarterTileWidth, y: startY - halfTileHeight - tileHeight, count: 4, nudge: 1 },
            { x: startX, y: startY - (2 * tileHeight), count: 5, nudge: 1 },
            { x: startX + tileWidth, y: startY - (2 * tileHeight), count: 5, nudge: -1 },
            { x: startX + tileWidth + threeQuarterTileWidth, y: startY - halfTileHeight - tileHeight, count: 4, nudge: -1 },
            { x: startX + tileWidth + (2 * threeQuarterTileWidth), y: startY - tileHeight, count: 3, nudge: -1 }
        ]
    };
}

function getRoadPlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    var halfRoadWidth = 14;
    var halfRoadHeight = 5;
    return [
        { x: startX - (2 * threeQuarterTileWidth) + halfTileWidth - halfRoadWidth, y: startY - tileHeight, imageIndex: 0, count: 4, deltaY: tileHeight - halfRoadHeight },
        { x: startX - (2 * threeQuarterTileWidth) + halfTileWidth, y: startY - tileHeight, imageIndex: 2, count: 3, deltaY: tileHeight }
    ];
}