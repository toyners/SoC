"use strict"

var tileHeight = 90;
var tileWidth = 90;
var halfTileHeight = Math.trunc(tileHeight / 2);
var halfTitleWidth = Math.trunc(tileWidth / 2);
var threeQuarterTileWidth = 68;

function getTilePlacementData(originX, originY) {
    var startX = originX - halfTitleWidth;
    var startY = originY - halfTileHeight;
    return [
        { x: startX - (2 * threeQuarterTileWidth), y: startY - halfTileHeight, count: 3 },
        { x: startX - threeQuarterTileWidth, y: startY - halfTileHeight - halfTileHeight, count: 4 },
        { x: startX, y: startY - (2 * halfTileHeight), count: 5 },
        { x: startX + threeQuarterTileWidth, y: startY - halfTileHeight - halfTileHeight, count: 4 },
        { x: startX + (2 * threeQuarterTileWidth), y: startY - halfTileHeight, count: 3 },
    ];
}

function getSettlementPlacementData(originX, originY) {
    var startX = originX - halfTitleWidth;
    var startY = originY - halfTileHeight;
    return [
        { x: startX - (2 * threeQuarterTileWidth), y: startY - tileHeight, count: 3, nudge: 1 },
        { x: startX - threeQuarterTileWidth, y: startY - halfTileHeight - tileHeight, count: 4, nudge: 1 },
        { x: startX, y: startY - (2 * tileHeight), count: 5, nudge: 1 },
        { x: startX + tileWidth, y: startY - (2 * tileHeight), count: 5, nudge: -1 },
        { x: startX + tileWidth + threeQuarterTileWidth, y: startY - halfTileHeight - tileHeight, count: 4, nudge: -1 },
        { x: startX + tileWidth + (2 * threeQuarterTileWidth), y: startY - tileHeight, count: 3, nudge: -1 },
    ];
}

function getRoadPlacementData(originX, originY) {
    var startX = originX - halfTitleWidth;
    var startY = originY - halfTileHeight;
    var halfRoadWidth = 14;
    var halfRoadHeight = 5;
    return [
        { x: startX - (2 * threeQuarterTileWidth) + halfTileWidth - halfRoadWidth, y: startY - tileHeight, imageIndex: 0, count: 4, deltaY: tileHeight - halfRoadHeight }
    ];
}