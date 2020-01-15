"use strict";

class InitialPlacementUI {
    constructor(state, textures, hoverImageIndex, settlementImageIndexById, confirmClickHandler, cancelSettlementClickHandler, cancelRoadClickHandler) {
        this.hoverImageIndex = hoverImageIndex;
        this.settlementImageIndexById = settlementImageIndexById;
        this.roadIconsById = {};
        this.roadsBySettlementId = {};
        this.roadsBySettlementLocation = {};
        this.selectedRoad = null;
        this.settlementId = null;
        this.settlementById = {};
        this.settlementByLocation = {};
        this.selectSettlementLabel = new Kiwi.GameObjects.Textfield(state, "Select a settlement", 10, 100, "#000", 22, 'normal', 'Impact');
        state.addChild(this.selectSettlementLabel);
        this.selectRoadLabel = new Kiwi.GameObjects.Textfield(state, "Select a road", 10, 132, "#000", 22, 'normal', 'Impact');
        state.addChild(this.selectRoadLabel);
        
        var buttonToggleHandler = function (context, params) {
            context.cellIndex = context.cellIndex == 0 ? 1 : 0;
        };

        this.confirmButton = new Kiwi.GameObjects.Sprite(state, textures.confirm, 10, 157);
        this.confirmButton.visible = false;
        this.confirmButton.input.onEntered.add(buttonToggleHandler, state);
        this.confirmButton.input.onLeft.add(buttonToggleHandler, state);
        this.confirmButton.input.onUp.add(confirmClickHandler, state);
        state.addChild(this.confirmButton);
        this.confirmed = false;

        this.cancelSettlementButton = new Kiwi.GameObjects.Sprite(state, textures.cancel, 190, 95);
        this.cancelSettlementButton.visible = false;
        this.cancelSettlementButton.input.onEntered.add(buttonToggleHandler, state);
        this.cancelSettlementButton.input.onLeft.add(buttonToggleHandler, state);
        this.cancelSettlementButton.input.onUp.add(cancelSettlementClickHandler, state);
        state.addChild(this.cancelSettlementButton);

        this.cancelRoadButton = new Kiwi.GameObjects.Sprite(state, textures.cancel, 190, 130);
        this.cancelRoadButton.visible = false;
        this.cancelRoadButton.input.onEntered.add(buttonToggleHandler, state);
        this.cancelRoadButton.input.onLeft.add(buttonToggleHandler, state);
        this.cancelRoadButton.input.onUp.add(cancelRoadClickHandler, state);
        state.addChild(this.cancelRoadButton);
    }

    addInitialPlacement(playerId, settlementLocation, endLocation) {
        var cellIndex = this.settlementImageIndexById[playerId];
        var settlement = this.settlementByLocation[settlementLocation];
        settlement.sprite.visible = true;
        settlement.sprite.cellIndex = cellIndex;
        for (var road of this.roadsBySettlementLocation[settlementLocation]) {
            if (road.location === endLocation) {
                road.icon.sprite.visible = true;
                road.icon.sprite.cellIndex = cellIndex;
            }
        }
    }

    addRoadForSettlement(roadIcon, settlementSpriteId, settlementLocation, endLocation) {
        var roads = this.roadsBySettlementId[settlementSpriteId];
        if (!roads) {
            roads = [];
            this.roadsBySettlementId[settlementSpriteId] = roads;
            this.roadsBySettlementLocation[settlementLocation] = roads;
        }
        roads.push({ location: endLocation, icon: roadIcon });
    }

    addRoadPlacement(roadSprite, defaultImageIndex, hoverImageIndex, firstSettlementSpriteId, firstSettlementLocation,
        secondSettlementSpriteId, secondSettlementLocation) {

        var roadIcon = {
            sprite: roadSprite,
            defaultImageIndex: defaultImageIndex,
            hoverImageIndex: hoverImageIndex            
        };

        this.roadIconsById[roadSprite.id] = roadIcon;

        this.addRoadForSettlement(roadIcon, firstSettlementSpriteId, firstSettlementLocation, secondSettlementLocation);
        this.addRoadForSettlement(roadIcon, secondSettlementSpriteId, secondSettlementLocation, firstSettlementLocation);
    }

    addSettlementSprite(settlementSprite, settlementLocation) {
        this.settlementById[settlementSprite.id] = { location: settlementLocation, sprite: settlementSprite };
        this.settlementByLocation[settlementLocation] = this.settlementById[settlementSprite.id];
    }

    getData() {
        return this.confirmed
            ? {
                settlementLocation: this.settlementById[this.settlementId].location,
                roadEndLocation: this.selectedRoad.location
            }
            : null;
    }

    isConfirmed() { return this.confirmed; }

    onCancelRoad() {
        if (this.selectedRoad) {
            this.selectedRoad.icon.sprite.cellIndex = 0;
            this.selectedRoad = null;
            this.showRoadSprites(this.settlementId);
            this.cancelRoadButton.visible = false;
            this.confirmButton.visible = false;
        }
    }

    onCancelSettlement() {
        this.onCancelRoad();

        for (var id in this.settlementById) {
            var settlement = this.settlementById[id];
            if (settlement.sprite.cellIndex === 0) {
                settlement.sprite.visible = true;
            }
            else if (this.settlementId === id) {
                settlement.sprite.cellIndex = 0;
                for (var road of this.roadsBySettlementId[this.settlementId]) {
                    road.icon.sprite.visible = false;
                }
            }
        }

        this.settlementId = null;
        this.cancelSettlementButton.visible = false;
        this.confirmButton.visible = false;
    }

    onConfirm() {
        this.confirmButton.visible = false;
        this.cancelRoadButton.visible = false;
        this.cancelSettlementButton.visible = false;
        this.selectSettlementLabel.visible = false;
        this.selectRoadLabel.visible = false;
        this.confirmed = true;
    }

    reset() { this.confirmed = false; }

    selectRoad() {
        for (var road of this.roadsBySettlementId[this.settlementId]) {
            if (road.icon.sprite.cellIndex === 0)
                road.icon.sprite.visible = false;
            else if (road.icon.sprite.cellIndex === this.hoverImageIndex)
                this.selectedRoad = road;
        }

        this.confirmButton.visible = true;
        this.cancelRoadButton.visible = true;
    }

    selectSettlement() {
        for (var id in this.settlementById) {
            var settlement = this.settlementById[id];
            if (settlement.sprite.cellIndex === 0)
                settlement.sprite.visible = false;
            else
                this.settlementId = id;
        }

        this.cancelSettlementButton.visible = true;
    }

    showRoadSprites(settlementId) {
        for (var road of this.roadsBySettlementId[settlementId]) {
            road.icon.sprite.visible = true;
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
        if (this.selectedRoad != null)
            return;
        var roadIcon = this.roadIconsById[spriteId];
        if (roadIcon.sprite.cellIndex === roadIcon.defaultImageIndex)
            roadIcon.sprite.cellIndex = roadIcon.hoverImageIndex;
        else if (roadIcon.sprite.cellIndex === roadIcon.hoverImageIndex)
            roadIcon.sprite.cellIndex = roadIcon.defaultImageIndex;
    }

    toggleSettlementSprite(spriteId) {
        if (this.settlementId != null)
            return;
        var settlement = this.settlementById[spriteId];
        if (settlement.sprite.cellIndex === 0 || settlement.sprite.cellIndex === this.hoverImageIndex)
            settlement.sprite.cellIndex = settlement.sprite.cellIndex === 0 ? this.hoverImageIndex : 0;
    }
}