"use strict";

class SettlementManager {
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

    confirmSettlement() {

    }

    highlightSettlement() {

    }

    selectSettlement() {

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
}
