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
var stepY = tileHeight - edgeHeight;

function getTilePlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    return {
        deltaY: stepY,
        data: [
            { x: startX - (2 * majorTileWidth), y: startY - stepY, count: 3 },
            { x: startX - majorTileWidth, y: startY - (stepY + majorTitleHeight), count: 4 },
            { x: startX, y: startY - (2 * stepY), count: 5 },
            { x: startX + majorTileWidth, y: startY - (stepY + majorTitleHeight), count: 4 },
            { x: startX + (2 * majorTileWidth), y: startY - stepY, count: 3 }
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
        // First column
        {
            x: startX - (2 * majorTileWidth) + settlementIndent,
            y: startY - (stepY - 1),
            offsets: [
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 }
            ]
        },
        // Second column
        {
            x: startX - majorTileWidth + settlementIndent,
            y: startY - (stepY + majorTitleHeight),
            offsets: [
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
            ]
        },
        // Third column
        {
            x: startX + settlementIndent,
            y: startY - (2 * stepY),
            offsets: [
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
            ]
        },
        // Fourth column
        {
            x: startX + tileWidth - settlementIndent,
            y: startY - (2 * stepY),
            offsets: [
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
            ]
        },
        // Fifth column
        {
            x: startX + tileWidth + majorTileWidth - settlementIndent,
            y: startY - (stepY + majorTitleHeight),
            offsets: [
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 },
            ]
        },
        // Sixth column
        {
            x: startX + tileWidth + (2 * majorTileWidth) - settlementIndent,
            y: startY - (stepY - 1),
            offsets: [
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 2 },
                { deltaX: settlementDeltaX, deltaY: settlementDeltaY - 1 },
                { deltaX: -settlementDeltaX, deltaY: settlementDeltaY - 1 }
            ]
        },
    ];
}

function getRoadPlacementData(originX, originY) {
    var startX = originX - halfTileWidth;
    var startY = originY - halfTileHeight;
    var angularRoads = 'roads';
    var defaultNorthEastIndex = 0;
    var hoverNorthEastIndex = 1;
    var defaultNorthWestIndex = 3;
    var hoverNorthWestIndex = 4;

    return [
        {
            imageName: angularRoads,
            imageIndex: defaultNorthEastIndex,
            hoverImageIndex: hoverNorthEastIndex,
            roads: [
                // Column 1
                { x: startX - (2 * majorTileWidth) + 2, y: startY - stepY + 6, locations: [0, 1] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + 6, locations: [2, 3] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + stepY + 6, locations: [4, 5] },

                // Column 2
                { x: startX - majorTileWidth + 2, y: startY - (stepY + majorTitleHeight) + 6, locations: [7, 8] },
                { x: startX - majorTileWidth + 2, y: startY - majorTitleHeight + 6, locations: [9, 10] },
                { x: startX - majorTileWidth + 1, y: startY + majorTitleHeight + 5, locations: [11, 12] },
                { x: startX - majorTileWidth + 1, y: startY + stepY + majorTitleHeight + 5, locations: [13, 14] },

                // Column 3
                { x: startX + 2, y: startY - (2 * stepY) + 5, locations: [16, 17] },
                { x: startX + 4, y: startY - stepY, locations: [18, 19] },

            ]
        },
        {
            imageName: angularRoads,
            imageIndex: defaultNorthWestIndex,
            hoverImageIndex: hoverNorthWestIndex,
            roads: [
                // Column 1
                { x: startX - (2 * majorTileWidth) + 2, y: startY - (stepY) + 53, locations: [1, 2] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + 53, locations: [3, 4] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + (stepY) + 53, locations: [5, 6] },

                // Column 2
                { x: startX - majorTileWidth + 2, y: startY - (stepY + majorTitleHeight) + 53, locations: [8, 9] },
                { x: startX - majorTileWidth + 2, y: startY - majorTitleHeight + 53, locations: [10, 11] },
                { x: startX - majorTileWidth + 2, y: startY + majorTitleHeight + 52, locations: [12, 13] },
                { x: startX - majorTileWidth + 2, y: startY + stepY + majorTitleHeight + 52, locations: [14, 15] },
            ]
        }
    ];
}