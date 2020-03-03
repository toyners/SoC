"use strict";

class InitialPlacementManager {
    constructor(gameState) {
        this.settlementHoverImageIndex = 1;
        this.imageIndexesByPlayerId = {};
        for (var playerId in gameState.playerData.playerById) {
            this.imageIndexesByPlayerId[playerId] = gameState.playerData.playerById[playerId].imageIndexes;
        }

        var x = 10;
        var y = 140;

        this.playersById = gameState.playersById;
        this.roadIconsById = {};
        this.roadsBySettlementId = {};
        this.roadsBySettlementLocation = {};
        this.selectedRoad = null;
        this.settlementId = null;
        this.settlementById = {};
        this.settlementByLocation = {};
        this.selectSettlementLabel = new Kiwi.GameObjects.Textfield(gameState, "Select a settlement", x, y, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.selectSettlementLabel);
        this.selectRoadLabel = new Kiwi.GameObjects.Textfield(gameState, "Select a road", x, y + 32, "#000", 20, 'normal', 'Impact');
        gameState.addChild(this.selectRoadLabel);

        var confirmButtonClickHandler = function (context, params) {
            if (!context.visible)
                return; 
            var gameState = context.parent;
            var previousPlayer = gameState.changeCurrentPlayer();
            gameState.currentPlayer.activate();
            previousPlayer.deactivate();
            
            gameState.playerActions.enqueue({
                gameId: gameState.gameId,
                id: gameState.playerId,
                type: 'PlaceSetupInfrastructureAction',
                data: {
                    initiatingPlayerId: gameState.playerId,
                    ...gameState.initialPlacementManager.getData()
                }
            });

            gameState.initialPlacementManager.reset();
            gameState.turnTimer.start();
        };

        this.confirmButton = new Kiwi.GameObjects.Sprite(gameState, gameState.textures.confirm, x, y + 57);
        this.confirmButton.visible = false;
        this.confirmButton.input.onEntered.add(gameState.buttonToggleHandler, gameState);
        this.confirmButton.input.onLeft.add(gameState.buttonToggleHandler, gameState);
        this.confirmButton.input.onUp.add(confirmButtonClickHandler, gameState);
        gameState.addChild(this.confirmButton);


        var cancelSettlementClickHandler = function (context, params) {
            if (!context.visible)
                return;

            context.parent.initialPlacementManager.cancelSettlementSelection();
        }

        this.cancelSettlementButton = new Kiwi.GameObjects.Sprite(gameState, gameState.textures.cancel, x + 170, y - 5);
        this.cancelSettlementButton.visible = false;
        this.cancelSettlementButton.input.onEntered.add(gameState.buttonToggleHandler, gameState);
        this.cancelSettlementButton.input.onLeft.add(gameState.buttonToggleHandler, gameState);
        this.cancelSettlementButton.input.onUp.add(cancelSettlementClickHandler, gameState);
        gameState.addChild(this.cancelSettlementButton);

        var cancelRoadClickHandler = function (context, params) {
            if (!context.visible)
                return;

            context.parent.initialPlacementManager.cancelRoadSelection();
        }

        this.cancelRoadButton = new Kiwi.GameObjects.Sprite(gameState, gameState.textures.cancel, x + 170, y + 30);
        this.cancelRoadButton.visible = false;
        this.cancelRoadButton.input.onEntered.add(gameState.buttonToggleHandler, gameState);
        this.cancelRoadButton.input.onLeft.add(gameState.buttonToggleHandler, gameState);
        this.cancelRoadButton.input.onUp.add(cancelRoadClickHandler, gameState);
        gameState.addChild(this.cancelRoadButton);

        this.placements = [];
    }

    addPlacement(playerId, settlementLocation, endLocation) {
        this.placements.push({ playerId: playerId, settlementLocation: settlementLocation, endLocation: endLocation });
    }

    showPlacements() {
        while (this.placements.length > 0) {
            var placement = this.placements.shift();

            var player = this.playersById[placement.playerId];

            var imageIndexes = this.imageIndexesByPlayerId[placement.playerId];
            var settlement = this.settlementByLocation[placement.settlementLocation];
            settlement.sprite.visible = true;
            settlement.sprite.cellIndex = imageIndexes[0];

            player.decrementSettlementCount();

            for (var road of this.roadsBySettlementLocation[placement.settlementLocation]) {
                if (road.location === placement.endLocation) {
                    road.icon.sprite.visible = true;
                    road.icon.sprite.cellIndex = imageIndexes[road.icon.typeIndex];
                    road.icon.sprite.input.enabled = false;

                    player.decrementRoadCount();
                }

                // Neighbouring settlement sprites are no longer valid for selection.
                var neighbouringSettlement = this.settlementByLocation[road.location];
                if (neighbouringSettlement) {
                    neighbouringSettlement.sprite.input.enabled = false;
                    delete this.settlementById[neighbouringSettlement.sprite.id];
                    delete this.settlementByLocation[road.location];
                }
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
        return {
            settlementLocation: this.settlementById[this.settlementId].location,
            roadEndLocation: this.selectedRoad.location
        };
    }

    isConfirmed() { return this.confirmed; }

    cancelRoadSelection() {
        if (this.selectedRoad) {
            this.selectedRoad.icon.sprite.cellIndex = this.selectedRoad.icon.defaultImageIndex;
            this.selectedRoad = null;
            this.showRoadSprites(this.settlementId);
            this.cancelRoadButton.visible = false;
            this.confirmButton.visible = false;
        }
    }

    cancelSettlementSelection() {
        this.cancelRoadSelection();

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

    reset() {
        this.confirmButton.visible = false;
        this.cancelRoadButton.visible = false;
        this.cancelSettlementButton.visible = false;
        this.selectSettlementLabel.visible = false;
        this.selectRoadLabel.visible = false;
        this.settlementId = null;
        this.selectedRoad = null;
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

    handleSettlementEnter(spriteId) {

        if (this.settlementId === spriteId) {
            for (var road of this.roadsBySettlementId[this.settlementId]) {
                if (road.icon.sprite.input.withinBounds)
                    road.icon.sprite.cellIndex = road.icon.defaultImageIndex;
            }
            return;
        }

        if (this.settlementId != null)
            return;

        var settlement = this.settlementById[spriteId];
        if (!settlement)
            return;
        if (settlement.sprite.cellIndex === 0)
            settlement.sprite.cellIndex = this.settlementHoverImageIndex;
    }

    handleSettlementLeft(spriteId) {

        if (this.settlementId === spriteId) {
            for (var road of this.roadsBySettlementId[this.settlementId]) {
                if (road.icon.sprite.input.withinBounds)
                    road.icon.sprite.cellIndex = road.icon.hoverImageIndex;
            }
            return;
        }

        if (this.settlementId != null)
            return;

        var settlement = this.settlementById[spriteId];
        if (!settlement)
            return;
        if (settlement.sprite.cellIndex === this.settlementHoverImageIndex)
            settlement.sprite.cellIndex = 0;
    }

    showRoadSprites(settlementId) {
        for (var road of this.roadsBySettlementId[settlementId]) {
            road.icon.sprite.visible = true;
        }
    }

    activate() {
        this.showPlacements();
        this.selectSettlementLabel.visible = true;
        this.selectRoadLabel.visible = true;
        for (var settlementKey in this.settlementById) {
            var settlement = this.settlementById[settlementKey];
            if (settlement.sprite.cellIndex === 0)
                settlement.sprite.visible = true;
        }
    }
}