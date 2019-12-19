"use strict"

var tileHeight = 100;
var tileWidth = 100;
var innerHeight = 88;
var innerWidth = 86;
var edgeHeight = 6;
var halfTileHeight = Math.trunc(tileHeight / 2);
var halfTileWidth = Math.trunc(tileWidth / 2);
var majorTileWidth = 70;
var majorTitleHeight = 48;

function getTilePlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    return {
        deltaY: tileHeight - edgeHeight,
        data: [
            { x: startX - (2 * majorTileWidth), y: startY - (tileHeight - edgeHeight), count: 3 },
            { x: startX - majorTileWidth, y: startY - (tileHeight - edgeHeight + majorTitleHeight), count: 4 },
            { x: startX, y: startY - (2 * (tileHeight - edgeHeight)), count: 5 },
            { x: startX + majorTileWidth, y: startY - (tileHeight - edgeHeight + majorTitleHeight), count: 4 },
            { x: startX + (2 * majorTileWidth), y: startY - (tileHeight - edgeHeight), count: 3 }
        ]
    };
}

var settlementIndent = 27;
var settlementDeltaX = 24;
var settlementDeltaY = 48;
function getSettlementPlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    return [
        {
            x: startX - (2 * majorTileWidth) + settlementIndent,
            y: startY - (tileHeight - edgeHeight - 1),
            offsets: [
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 }
            ]
        }
            /*{ x: startX - majorTileWidth + settlementIndent, y: startY - (tileHeight - edgeHeight + majorTitleHeight), count: 9, direction: -1 },
            { x: startX + settlementIndent, y: startY - (2 * (tileHeight - edgeHeight)), count: 11, direction: -1 },

            { x: startX + tileWidth, y: startY - (2 * tileHeight), count: 11, direction: -1 },
            { x: startX + tileWidth + majorTileWidth, y: startY - halfTileHeight - tileHeight, count: 9, direction: -1 },
            { x: startX + tileWidth + (2 * majorTileWidth), y: startY - tileHeight, count: 7, direction: -1 }*/
    ];
}

function getRoadPlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    return [
        {
            imageName: 'roads',
            imageIndex: 0,
            hoverImageIndex: 1,
            roads: [
                //{ x: startX + 2, y: startY + 4, locations: [0, 1] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY - (tileHeight - edgeHeight) + 6, locations: [0, 1] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + 6, locations: [2, 3] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + (tileHeight - edgeHeight) + 6, locations: [4, 5] },

                { x: startX - majorTileWidth + 2, y: startY - (tileHeight - edgeHeight + majorTitleHeight) + 6, locations: [7, 8] },
                { x: startX - majorTileWidth + 2, y: startY - majorTitleHeight + 6, locations: [9, 10] },
                { x: startX - majorTileWidth + 2, y: startY + (tileHeight - edgeHeight + majorTitleHeight) + 6, locations: [11, 12] },
                { x: startX - majorTileWidth + 2, y: startY + ((2 * (tileHeight - edgeHeight)) + majorTitleHeight) + 6, locations: [13, 14] }
            ]
        },
        {
            imageName: 'roads',
            imageIndex: 3,
            hoverImageIndex: 4,
            roads: [
                //{ x: startX + 2, y: startY + 53, locations: [1, 2] }
                { x: startX - (2 * majorTileWidth) + 2, y: startY - (tileHeight - edgeHeight) + 53, locations: [1, 2] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + 53, locations: [3, 4] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + (tileHeight - edgeHeight) + 53, locations: [5, 6] },

                { x: startX - majorTileWidth + 2, y: startY - (tileHeight - edgeHeight + majorTitleHeight) + 53, locations: [8, 9] },
            ]
        }
    ];
}