"use strict";

class InitialPlacementManager {
    constructor(state, textures, imageIndexesById, confirmClickHandler, cancelSettlementClickHandler, cancelRoadClickHandler) {
        this.settlementHoverImageIndex = 1;
        this.roundCount = 0;
        this.imageIndexesById = imageIndexesById;
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

    addPlacement(playerId, settlementLocation, endLocation) {
        var imageIndexes = this.imageIndexesById[playerId];
        var settlement = this.settlementByLocation[settlementLocation];
        settlement.sprite.visible = true;
        settlement.sprite.cellIndex = imageIndexes[0];
        for (var road of this.roadsBySettlementLocation[settlementLocation]) {
            if (road.location === endLocation) {
                road.icon.sprite.visible = true;
                road.icon.sprite.cellIndex = imageIndexes[road.icon.typeIndex];
                road.icon.sprite.onUp = [];
                road.icon.sprite.onEntered = [];
                road.icon.sprite.onLeft = [];
            }

            // Neighbouring settlement sprites are no longer valid for selection.
            var neighbouringSettlement = this.settlementByLocation[road.location];
            if (neighbouringSettlement) {
                delete this.settlementById[neighbouringSettlement.sprite.id];
                delete this.settlementByLocation[road.location];
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

    addRoadPlacement(roadSprite, defaultImageIndex, hoverImageIndex, type, firstSettlementSpriteId, firstSettlementLocation,
        secondSettlementSpriteId, secondSettlementLocation) {

        var roadIcon = {
            sprite: roadSprite,
            defaultImageIndex: defaultImageIndex,
            hoverImageIndex: hoverImageIndex,
            typeIndex: type
        };

        this.roadIconsById[roadSprite.id] = roadIcon;

        this.addRoadForSettlement(roadIcon, firstSettlementSpriteId, firstSettlementLocation, secondSettlementLocation);
        this.addRoadForSettlement(roadIcon, secondSettlementSpriteId, secondSettlementLocation, firstSettlementLocation);
    }

    addSettlementSprite(settlementSprite, settlementLocation) {
        var settlement = { location: settlementLocation, sprite: settlementSprite };
        this.settlementById[settlementSprite.id] = settlement;
        this.settlementByLocation[settlementLocation] = settlement;
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

    isLastRound() { return this.roundCount < 3; }

    onCancelRoad() {
        if (this.selectedRoad) {
            this.selectedRoad.icon.sprite.cellIndex = this.selectedRoad.icon.defaultImageIndex;
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

    reset() {
        this.settlementId = null;
        this.selectedRoad = null;
        this.selectSettlementLabel.visible = true;
        this.selectRoadLabel.visible = true;
        this.confirmed = false;
    }

    handleRoadClick(spriteId) {
        if (!this.settlementId || this.selectedRoad)
            return;

        for (var road of this.roadsBySettlementId[this.settlementId]) {
            if (road.icon.sprite.id !== spriteId) {
                road.icon.sprite.visible = false;
            }
            else {
                road.icon.sprite.cellIndex = road.icon.hoverImageIndex;
                this.selectedRoad = road;
            }
        }

        this.confirmButton.visible = true;
        this.cancelRoadButton.visible = true;
    }

    handleRoadHoverEnter(spriteId) {
        if (!this.settlementId || this.selectedRoad)
            return;

        var roadIcon = this.roadIconsById[spriteId];
        if (roadIcon.sprite.cellIndex === roadIcon.defaultImageIndex)
            roadIcon.sprite.cellIndex = roadIcon.hoverImageIndex;
        
    }

    handleRoadHoverLeft(spriteId) {
        if (!this.settlementId || this.selectedRoad)
            return;

        var roadIcon = this.roadIconsById[spriteId];
        if (roadIcon.sprite.cellIndex === roadIcon.hoverImageIndex)
            roadIcon.sprite.cellIndex = roadIcon.defaultImageIndex;
    }

    handleSettlementClick(spriteId) {
        if (this.settlementId)
            return;

        for (var id in this.settlementById) {
            var settlementSprite = this.settlementById[id].sprite;
            if (id != spriteId && settlementSprite.cellIndex === 0) {
                settlementSprite.visible = false;
            } else if (id == spriteId) {
                this.settlementId = spriteId;
            }
        }

        this.showRoadSprites(this.settlementId);
        this.cancelSettlementButton.visible = true;
    }

    handleSettlementHover(spriteId) {
        if (this.settlementId != null)
            return;

        var settlement = this.settlementById[spriteId];
        if (!settlement)
            return;
        if (settlement.sprite.cellIndex === 0 || settlement.sprite.cellIndex === this.settlementHoverImageIndex)
            settlement.sprite.cellIndex = settlement.sprite.cellIndex === 0 ? this.settlementHoverImageIndex : 0;
    }

    showRoadSprites(settlementId) {
        for (var road of this.roadsBySettlementId[settlementId]) {
            road.icon.sprite.visible = true;
        }
    }

    showSettlementSprites() {
        this.roundCount += 1;
        for (var settlementKey in this.settlementById) {
            var settlement = this.settlementById[settlementKey];
            if (settlement.sprite.cellIndex === 0)
                settlement.sprite.visible = true;
        }
    }
}