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

    return [
        {
            imageName: 'roads',
            imageIndex: 0,
            hoverImageIndex: 1,
            roads: [
                // Column 1
                { x: startX - (2 * majorTileWidth) + 2, y: startY - stepY + 6, locations: [0, 1] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + 6, locations: [2, 3] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + stepY + 6, locations: [4, 5] },

                // Column 2
                { x: startX - majorTileWidth + 2, y: startY - (stepY + majorTitleHeight) + 6, locations: [7, 8] },
                { x: startX - majorTileWidth + 2, y: startY - majorTitleHeight + 6, locations: [9, 10] },
                { x: startX - majorTileWidth + 2, y: startY + (stepY + majorTitleHeight) + 6, locations: [11, 12] },
                { x: startX - majorTileWidth + 2, y: startY + ((2 * stepY) + majorTitleHeight) + 6, locations: [13, 14] },

                // Column 3
                { x: startX, y: startY - (2 * stepY), locations: [16, 17] },

            ]
        },
        {
            imageName: 'roads',
            imageIndex: 3,
            hoverImageIndex: 4,
            roads: [
                // Column 1
                { x: startX - (2 * majorTileWidth) + 2, y: startY - (stepY) + 53, locations: [1, 2] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + 53, locations: [3, 4] },
                { x: startX - (2 * majorTileWidth) + 2, y: startY + (stepY) + 53, locations: [5, 6] },

                { x: startX - majorTileWidth + 2, y: startY - (stepY + majorTitleHeight) + 53, locations: [8, 9] },
            ]
        }
    ];
}