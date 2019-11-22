"use strict";

class InitialRoadPlacementUI {
    constructor() {
        this.roadsBySpriteId = {}; 
        this.roadsBySettlementId = {};
        this.settlementId = null;
        this.roadSelected = false;
    }

    addRoadPlacement(roadSprite, roadHoverSprite) {
        var road = { default: roadSprite, hover: roadHoverSprite };
        this.roadsBySpriteId[roadSprite.id] = road;
        this.roadsBySpriteId[roadHoverSprite.id] = road;
    }

    addRoadForSettlement(roadSprite, settlementSpriteId) {
        var roadSprites = this.roadsBySettlementId[settlementSpriteId];
        if (!roadSprites) {
            roadSprites = [];
            this.roadsBySettlementId[settlementSpriteId] = roadSprites;
        }
        roadSprites.push(roadSprite);
    }

    clearRoad() {
        for (var road of this.roadsBySettlementId[this.settlementId]) {
            road.visible = true;
        }

        this.roadSelected = false;
    }

    selectRoad() {
        for (var road of this.roadsBySettlementId[this.settlementId]) {
            road.visible = false;
        }

        this.roadSelected = true;
    }

    showRoadSprites(spriteId) {
        for (var road of this.roadsBySettlementId[spriteId]) {
            road.visible = true;
        }
        this.settlementId = spriteId;
    }

    toggle(spriteId) {
        var roadData = this.roadsBySpriteId[spriteId];
        if (spriteId === roadData.default.id) {
            //roadData.default.visible = false;
            roadData.hover.visible = true;
        } else {
            //roadData.default.visible = true;
            roadData.hover.visible = false;
        }
    }
}