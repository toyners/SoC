"use strict";

class SettlementPlacementUI {
    constructor() {
        this.locked = false;
        this.settlementPlacements = {};
        this.settlementIconSpritesById = {};
    }

    addSettlementPlacement(settlementIconSprite, settlementHoverSprite) {
        this.settlementPlacements[settlementIconSprite.id] = { icon: settlementIconSprite, hover: settlementHoverSprite };
        this.settlementPlacements[settlementHoverSprite.id] = this.settlementPlacements[settlementIconSprite.id];
        this.settlementIconSpritesById[settlementIconSprite.id] = settlementIconSprite;
    }

    clearSettlement() {
        for (var id in this.settlementIconSpritesById) {
            this.settlementIconSpritesById[id].visible = true;
        }

        this.locked = false;
    }

    selectSettlement() {
        for (var id in this.settlementIconSpritesById) {
            this.settlementIconSpritesById[id].visible = false;
        }

        this.locked = true;
    }

    toggleSettlementSprite(spriteId) {
        if (this.locked)
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

    getSettlementLocation(settlementIconSpriteId) {

    }
}
