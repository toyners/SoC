"use strict";

class InitialPlacementUI {
    constructor() {
        this.settlementSelected = false;
        this.settlementPlacements = {};
        this.settlementIconSpritesById = {};
    }

    toggleSettlementSprite(spriteId) {
        if (this.settlementSelected)
            return;
        var settlementPlacement = this.settlementPlacements[spriteId];
        if (spriteId === settlementPlacement.icon.id) {
            settlementPlacement.icon.visible = false;
            settlementPlacement.hover.visible = true;
        } else {
            settlementPlacement.icon.visible = true;
            settlementPlacement.hover.visible = false;
        }
    }
}