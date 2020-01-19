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
    var angularRoads = 'angular_roads';
    var horizontalRoads = 'horizontal_roads'
    var defaultNorthEastIndex = 0;
    var hoverNorthEastIndex = 1;
    var defaultNorthWestIndex = 3;
    var hoverNorthWestIndex = 4;
    var defaultHorizontalIndex = 0;
    var hoverHorizontalIndex = 1;
    var northEastType = 1;
    var northWestType = 2;
    var horizontalType = 3;
    return [
        {
            imageName: angularRoads,
            imageIndex: defaultNorthEastIndex,
            hoverImageIndex: hoverNorthEastIndex,
            type: northEastType,
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
                { x: startX + 2, y: startY - stepY + 5, locations: [18, 19] },
                { x: startX + 2, y: startY + 5, locations: [20, 21] },
                { x: startX + 2, y: startY + stepY + 5, locations: [22, 23] },
                { x: startX + 2, y: startY + (2 * stepY) + 5, locations: [24, 25] },

                // Column 4
                { x: startX + majorTileWidth + 2, y: startY - (2 * stepY) + 52, locations: [28, 29] },
                { x: startX + majorTileWidth + 2, y: startY - stepY + 52, locations: [30, 31] },
                { x: startX + majorTileWidth + 2, y: startY + 52, locations: [32, 33] },
                { x: startX + majorTileWidth + 2, y: startY + stepY + 52, locations: [34, 35] },
                { x: startX + majorTileWidth + 2, y: startY + (2 * stepY) + 52, locations: [36, 37] },

                // Column 5
                { x: startX + (2 * majorTileWidth) + 2, y: startY - stepY + 6, locations: [39, 40] },
                { x: startX + (2 * majorTileWidth) + 2, y: startY + 6, locations: [41, 42] },
                { x: startX + (2 * majorTileWidth) + 2, y: startY + stepY + 6, locations: [43, 44] },
                { x: startX + (2 * majorTileWidth) + 2, y: startY + (2 * stepY) + 6, locations: [45, 46] },

                // Column 6
                { x: startX + (3 * majorTileWidth) + 2, y: startY - majorTitleHeight + 6, locations: [48, 49] },
                { x: startX + (3 * majorTileWidth) + 2, y: startY + majorTitleHeight + 5, locations: [50, 51] },
                { x: startX + (3 * majorTileWidth) + 2, y: startY + stepY + majorTitleHeight + 5, locations: [52, 53] },
            ]
        },
        {
            imageName: angularRoads,
            imageIndex: defaultNorthWestIndex,
            hoverImageIndex: hoverNorthWestIndex,
            type: northWestType,
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

                // Column 3
                { x: startX + 2, y: startY - (2 * stepY) + 53, locations: [17, 18] },
                { x: startX + 2, y: startY - stepY + 53, locations: [19, 20] },
                { x: startX + 2, y: startY + 53, locations: [21, 22] },
                { x: startX + 2, y: startY + stepY + 53, locations: [23, 24] },
                { x: startX + 2, y: startY + (2 * stepY) + 53, locations: [25, 26] },

                // Column 4
                { x: startX + majorTileWidth + 2, y: startY - (2 * stepY) + 5, locations: [27, 28] },
                { x: startX + majorTileWidth + 2, y: startY - stepY + 5, locations: [29, 30] },
                { x: startX + majorTileWidth + 2, y: startY + 5, locations: [31, 32] },
                { x: startX + majorTileWidth + 2, y: startY + stepY + 5, locations: [33, 34] },
                { x: startX + majorTileWidth + 2, y: startY + (2 * stepY) + 5, locations: [35, 36] },

                // Column 5
                { x: startX + (2 * majorTileWidth) + 2, y: startY - (2 * stepY) + 53, locations: [38, 39] },
                { x: startX + (2 * majorTileWidth) + 2, y: startY - stepY + 53, locations: [40, 41] },
                { x: startX + (2 * majorTileWidth) + 2, y: startY + 53, locations: [42, 43] },
                { x: startX + (2 * majorTileWidth) + 2, y: startY + stepY + 53, locations: [44, 45] },

                // Column 6
                { x: startX + (3 * majorTileWidth) + 2, y: startY - (stepY + majorTitleHeight) + 53, locations: [47, 48] },
                { x: startX + (3 * majorTileWidth) + 2, y: startY - majorTitleHeight + 53, locations: [49, 50] },
                { x: startX + (3 * majorTileWidth) + 2, y: startY + majorTitleHeight + 53, locations: [51, 52] },
            ]
        },
        {
            imageName: horizontalRoads,
            imageIndex: defaultHorizontalIndex,
            hoverImageIndex: hoverHorizontalIndex,
            type: horizontalType,
            roads: [
                // Column 1
                { x: startX - (2 * majorTileWidth) + 31, y: startY - stepY - 2, locations: [0, 8] },
                { x: startX - (2 * majorTileWidth) + 31, y: startY - 2, locations: [2, 10] },
                { x: startX - (2 * majorTileWidth) + 31, y: startY + stepY - 2, locations: [4, 12] },
                { x: startX - (2 * majorTileWidth) + 31, y: startY + (2 * stepY) - 2, locations: [6, 14] },

                // Column 2
                { x: startX - majorTileWidth + 31, y: startY - (stepY + majorTitleHeight) - 2, locations: [7, 17] },
                { x: startX - majorTileWidth + 31, y: startY - majorTitleHeight - 2, locations: [9, 19] },
                { x: startX - majorTileWidth + 31, y: startY + majorTitleHeight - 4, locations: [11, 21] },
                { x: startX - majorTileWidth + 31, y: startY + (stepY + majorTitleHeight) - 4, locations: [13, 23] },
                { x: startX - majorTileWidth + 31, y: startY + ((2 * stepY) + majorTitleHeight) - 4, locations: [15, 25] },

                // Column 3
                { x: startX + 31, y: startY - (2 * stepY) - 2, locations: [16, 27] },
                { x: startX + 31, y: startY - stepY - 2, locations: [18, 29] },
                { x: startX + 31, y: startY - 2, locations: [20, 31] },
                { x: startX + 31, y: startY + stepY - 2, locations: [22, 33] },
                { x: startX + 31, y: startY + (2 * stepY) - 2, locations: [24, 35] },
                { x: startX + 31, y: startY + (3 * stepY) - 2, locations: [26, 37] },

                // Column 4
                { x: startX + majorTileWidth + 31, y: startY - (stepY + majorTitleHeight) - 2, locations: [28, 38] },
                { x: startX + majorTileWidth + 31, y: startY - majorTitleHeight - 2, locations: [30, 40] },
                { x: startX + majorTileWidth + 31, y: startY + majorTitleHeight - 4, locations: [32, 42] },
                { x: startX + majorTileWidth + 31, y: startY + (stepY + majorTitleHeight) - 4, locations: [34, 44] },
                { x: startX + majorTileWidth + 31, y: startY + ((2 * stepY) + majorTitleHeight) - 4, locations: [36, 46] },

                // Column 5
                { x: startX + (2 * majorTileWidth) + 31, y: startY - stepY - 2, locations: [39, 47] },
                { x: startX + (2 * majorTileWidth) + 31, y: startY - 2, locations: [41, 49] },
                { x: startX + (2 * majorTileWidth) + 31, y: startY + stepY - 2, locations: [43, 51] },
                { x: startX + (2 * majorTileWidth) + 31, y: startY + (2 * stepY) - 2, locations: [45, 53] },
            ]
        }
    ];
}