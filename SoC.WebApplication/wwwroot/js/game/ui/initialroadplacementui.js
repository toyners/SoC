"use strict";

class InitialRoadPlacementUI {
    constructor() {
        this.roadsBySettlementId = {};
    }

    addRoadPlacement(spriteId, roadIcons) {
        this.roadsBySettlementId[spriteId, roadIcons];
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

    toggleRoadSprites(spriteId) {
        for (road of this.roadsBySettlementId[spriteId]) {
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
}