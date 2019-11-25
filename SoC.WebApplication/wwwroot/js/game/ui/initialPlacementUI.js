"use strict";

class InitialPlacementUI {
    constructor() {
        this.roadsBySpriteId = {};
        this.roadsBySettlementId = {};
        this.roadSelected = false;
        this.settlementId = null;
        this.settlementSelected = false;
        this.settlementSpritesById = {};
    }

    addRoadPlacement(roadSprite) {
        this.roadsBySpriteId[roadSprite.id] = roadSprite;
    }

    addSettlementSprite(settlementSprite) {
        this.settlementSpritesById[settlementSprite.id] = settlementSprite;
    }

    selectRoad() {
        for (var roadSprite of this.roadsBySettlementId[this.settlementId]) {
            if (roadSprite.cellIndex === 0)
                roadSprite.visible = false;
        }

        this.roadSelected = true;
    }

    selectSettlement() {
        for (var id in this.settlementIconSpritesById) {
            var settlementSprite = this.settlementIconSpritesById[id];
            if (settlementSprite.cellIndex === 0)
                settlementSprite.visible = false;
        }

        this.settlementSelected = true;
    }

    showRoadSprites(spriteId) {
        for (var road of this.roadsBySettlementId[spriteId]) {
            road.visible = true;
        }
        this.settlementId = spriteId;
    }

    toggleRoadSprite(spriteId) {
        if (!this.roadSelected) {
            var roadSprite = this.roadsBySpriteId[spriteId];
            roadSprite.cellIndex = roadSprite.cellIndex === 0 ? 1 : 0;
        }
    }

    toggleSettlementSprite(spriteId) {
        if (!this.settlementSelected) {
            var settlementSprite = this.settlementSpritesById[spriteId];
            settlementSprite.cellIndex = settlementSprite.cellIndex === 0 ? 1 : 0;
        }
    }
}