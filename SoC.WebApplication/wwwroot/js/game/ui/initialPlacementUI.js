"use strict";

class InitialPlacementUI {
    constructor(state, textures) {
        this.roadsBySpriteId = {};
        this.roadsBySettlementId = {};
        this.roadLocation = null;
        this.settlementId = null;
        this.settlementById = {};
        this.selectSettlementLabel = new Kiwi.GameObjects.Textfield(state, "Select a settlement", 10, 100, "#000", 22, 'normal', 'Impact');
        state.addChild(this.selectSettlementLabel);
        this.selectRoadLabel = new Kiwi.GameObjects.Textfield(state, "Select a road", 10, 120, "#000", 22, 'normal', 'Impact');
        state.addChild(this.selectRoadLabel);
        this.confirmButton = new Kiwi.GameObjects.Sprite(state, textures.confirm, 30, 150);
        this.confirmButton.visible = false;

        var confirmToggleHandler = function (context, params) {
            context.cellIndex = context.cellIndex == 0 ? 1 : 0;
        };

        this.confirmButton.input.onEntered.add(confirmToggleHandler, state);
        this.confirmButton.input.onLeft.add(confirmToggleHandler, state);
        state.addChild(this.confirmButton);
        this.confirmed = false;
    }

    addRoadForSettlement(roadSprite, settlementSpriteId, endLocation) {
        var roads = this.roadsBySettlementId[settlementSpriteId];
        if (!roads) {
            roads = [];
            this.roadsBySettlementId[settlementSpriteId] = roads;
        }
        roads.push({ location: endLocation, sprite: roadSprite });
    }

    addRoadPlacement(roadSprite) {
        this.roadsBySpriteId[roadSprite.id] = roadSprite;
    }

    addSettlementSprite(settlementSprite, settlementLocation) {
        this.settlementById[settlementSprite.id] = { location: settlementLocation, sprite: settlementSprite };
    }

    getData() {
        return this.confirmed
            ? {
                settlementLocation: this.settlementById[this.settlementId].location,
                roadEndLocation: this.roadLocation
            }
            : null;
    }

    onConfirm() {
        this.confirmButton.visible = true;
        this.confirmed = true;
    }

    selectRoad() {
        for (var road of this.roadsBySettlementId[this.settlementId]) {
            if (road.sprite.cellIndex === 0)
                road.sprite.visible = false;
            else
                this.roadLocation = road.location;
        }

        this.confirmButton.visible = true;
    }

    selectSettlement() {
        for (var id in this.settlementById) {
            var settlement = this.settlementById[id];
            if (settlement.sprite.cellIndex === 0)
                settlement.sprite.visible = false;
            else
                this.settlementId = id;
        }
    }

    showRoadSprites(spriteId) {
        for (var road of this.roadsBySettlementId[spriteId]) {
            road.sprite.visible = true;
        }
    }

    toggleRoadSprite(spriteId) {
        if (this.roadLocation != null)
            return;
        var roadSprite = this.roadsBySpriteId[spriteId];
        roadSprite.cellIndex = roadSprite.cellIndex === 0 ? 1 : 0;
    }

    toggleSettlementSprite(spriteId) {
        if (this.settlementId != null)
            return;
        var settlement = this.settlementById[spriteId];
        settlement.sprite.cellIndex = settlement.sprite.cellIndex === 0 ? 1 : 0;
    }
}