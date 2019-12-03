"use strict";

class InitialPlacementUI {
    constructor(state, textures, settlementImageIndexById) {
        this.settlementImageIndexById = settlementImageIndexById;
        this.roadsBySpriteId = {};
        this.roadsBySettlementId = {};
        this.roadsBySettlementLocation = {};
        this.roadLocation = null;
        this.settlementId = null;
        this.settlementById = {};
        this.settlementByLocation = {};
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

        this.cancelSettlementButton = new Kiwi.GameObjects.Sprite(state, textures.cancel, 70, 100);
        this.cancelSettlementButton.visible = false;
        this.cancelRoadButton = new Kiwi.GameObjects.Sprite(state, textures.cancel, 70, 120);
    }

    addInitialPlacement(playerId, settlementLocation, endLocation) {
        var cellIndex = this.settlementImageIndexById[playerId];
        var settlement = this.settlementByLocation[settlementLocation];
        settlement.sprite.visible = true;
        settlement.sprite.cellIndex = cellIndex;
        for (var road of this.roadsBySettlementLocation[settlementLocation]) {
            if (road.location === endLocation) {
                road.sprite.visible = true;
                road.sprite.cellIndex = cellIndex;
            }
        }
    }

    addRoadForSettlement(roadSprite, settlementSpriteId, settlementLocation, endLocation) {
        var roads = this.roadsBySettlementId[settlementSpriteId];
        if (!roads) {
            roads = [];
            this.roadsBySettlementId[settlementSpriteId] = roads;
            this.roadsBySettlementLocation[settlementLocation] = roads;
        }
        roads.push({ location: endLocation, sprite: roadSprite });

    }

    addRoadPlacement(roadSprite) {
        this.roadsBySpriteId[roadSprite.id] = roadSprite;
    }

    addSettlementSprite(settlementSprite, settlementLocation) {
        this.settlementById[settlementSprite.id] = { location: settlementLocation, sprite: settlementSprite };
        this.settlementByLocation[settlementLocation] = this.settlementById[settlementSprite.id];
    }

    getData() {
        return this.confirmed
            ? {
                settlementLocation: this.settlementById[this.settlementId].location,
                roadEndLocation: this.roadLocation
            }
            : null;
    }

    isConfirmed() { return this.confirmed; }

    onConfirm() {
        this.confirmButton.visible = false;
        this.selectSettlementLabel.visible = false;
        this.selectRoadLabel.visible = false;
        this.confirmed = true;
    }

    reset() { this.confirmed = false; }

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

    showRoadSprites(settlementId) {
        for (var road of this.roadsBySettlementId[settlementId]) {
            road.sprite.visible = true;
        }
    }

    showSettlementSprites() {
        for (var settlementKey in this.settlementById) {
            var settlement = this.settlementById[settlementKey];
            if (settlement.sprite.cellIndex === 0)
                settlement.sprite.visible = true;
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