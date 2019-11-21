"use strict";

class InitialRoadPlacementUI {
    constructor() {
        this.roadsBySpriteId = {}; 
        this.roadsBySettlementId = {};
    }

    addRoadPlacement(roadSprite, roadHoverSprite) {
        var road = { default: roadSprite, hover: roadHoverSprite };
        this.roadsBySpriteId[roadSprite.id] = road;
        this.roadsBySpriteId[roadHoverSprite.id] = road;
    }

    addRoadPlacement(roadSprite, settlementSpriteId) {
        var roadSprites = this.roadsBySettlementId[settlementSpriteId];
        if (!roadSprites) {
            roadSprites = [];
            this.roadsBySettlementId[settlementSpriteId] = roadSprites;
        }
        roadSprites.push(roadSprite);
    }

    clearRoad() {
        /*for (var id in this.settlementIconSpritesById) {
            this.settlementIconSpritesById[id].visible = true;
        }

        this.locked = false;*/
    }

    selectRoad() {
        /*for (var id in this.settlementIconSpritesById) {
            this.settlementIconSpritesById[id].visible = false;
        }

        this.locked = true;*/
    }

    showRoadSprites(spriteId) {
        for (var road of this.roadsBySettlementId[spriteId]) {
            road.visible = true;
        }
        /*if (this.locked)
            return;
        var settlementPlacement = this.settlementPlacements[spriteId];
        if (spriteId === settlementPlacement.icon.id) {
            settlementPlacement.icon.visible = false;
            settlementPlacement.hover.visible = true;
        } else {
            settlementPlacement.icon.visible = true;
            settlementPlacement.hover.visible = false;
        }*/
    }

    toggle(spriteId) {

    }
}