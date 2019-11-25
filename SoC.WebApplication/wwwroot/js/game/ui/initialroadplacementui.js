"use strict";

class InitialRoadPlacementUI {
    constructor() {
        this.roadsBySpriteId = {}; 
        this.roadsBySettlementId = {};
        this.settlementId = null;
        this.roadSelected = false;
    }

    addRoadPlacement(roadSprite) {
        this.roadsBySpriteId[roadSprite.id] = roadSprite;
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
        for (var roadSprite of this.roadsBySettlementId[this.settlementId]) {
            roadSprite.cellIndex = 0;
            roadSprite.visible = true;
        }

        this.roadSelected = false;
    }

    selectRoad() {
        for (var roadSprite of this.roadsBySettlementId[this.settlementId]) {
            if (roadSprite.cellIndex === 0)
                roadSprite.visible = false;
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
        if (!this.roadSelected) {
            var roadSprite = this.roadsBySpriteId[spriteId];
            roadSprite.cellIndex = roadSprite.cellIndex === 0 ? 1 : 0;
        }
    }
}