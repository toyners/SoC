"use strict";

class InitialPlacementUI {
    constructor(state, textures) {
        this.roadsBySpriteId = {};
        this.roadsBySettlementId = {};
        this.roadSelected = false;
        this.settlementId = null;
        this.settlementSelected = false;
        this.settlementSpritesById = {};
        this.selectSettlementLabel = new Kiwi.GameObjects.Textfield(state, "Select a settlement", 10, 100, "#000", 22, 'normal', 'Impact');
        state.addChild(this.selectSettlementLabel);
        this.selectRoadLabel = new Kiwi.GameObjects.Textfield(state, "Select a road", 10, 120, "#000", 22, 'normal', 'Impact');
        state.addChild(this.selectRoadLabel);
        this.confirmButton = new Kiwi.GameObjects.Sprite(state, textures.confirm, 30, 150);
        this.confirmButton.visible = false;
        
        this.confirmButton.input.onEntered.add(function (context, params) { context.cellIndex = 1; }, state);
        this.confirmButton.input.onLeft.add(function (context, params) { context.cellIndex = 0; }, state);
        state.addChild(this.confirmButton);
        this.confirmed = false;
    }

    addRoadForSettlement(roadSprite, settlementSpriteId) {
        var roadSprites = this.roadsBySettlementId[settlementSpriteId];
        if (!roadSprites) {
            roadSprites = [];
            this.roadsBySettlementId[settlementSpriteId] = roadSprites;
        }
        roadSprites.push(roadSprite);
    }

    addRoadPlacement(roadSprite) {
        this.roadsBySpriteId[roadSprite.id] = roadSprite;
    }

    addSettlementSprite(settlementSprite) {
        this.settlementSpritesById[settlementSprite.id] = settlementSprite;
    }

    getData() {
        return this.confirmed
            ? { 
                settlementLocation: this.settlementLocation,
                roadEndLocation: this.roadEndLocation
            }
            : null;
    }

    onConfirm() {
        this.confirmButton.visible = true;
        this.confirmed = true;
    }

    selectRoad() {
        for (var roadSprite of this.roadsBySettlementId[this.settlementId]) {
            if (roadSprite.cellIndex === 0)
                roadSprite.visible = false;
        }

        this.roadSelected = true;
        this.confirmButton.visible = true;
    }

    selectSettlement() {
        for (var id in this.settlementSpritesById) {
            var settlementSprite = this.settlementSpritesById[id];
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