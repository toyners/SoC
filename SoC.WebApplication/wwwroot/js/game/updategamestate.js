"use strict"

function updateGameState() {
    Kiwi.State.prototype.update.call(this); // Must use .call otherwise the components property is undefined 

    if (!this.gameEvents.isEmpty()) {
        var gameEvent = this.gameEvents.dequeue();
        switch (gameEvent.typeName) {
            case "ConfirmGameStartEvent": {
                this.playerActions.enqueue({
                    gameId: this.gameId,
                    id: this.playerId,
                    type: 'ConfirmGameStartAction',
                    data: {
                        initiatingPlayerId: this.playerId
                    }
                });
                break;
            }
            case "PlaceRobberEvent": {
                break;
            }
            case "PlaceSetupInfrastructureEvent": {
                if (!this.currentPlayer) {
                    this.changeCurrentPlayer();
                    this.currentPlayer.activate();
                }
                if (this.currentPlayer.isLocal) {
                    this.initialPlacementManager.activate();
                }
                break;
            }
            case "ResourcesCollectedEvent": {
                processCollectedResources(this, gameEvent.resourcesCollectedByPlayerId);
                break;
            }
            case "SetupInfrastructurePlacedEvent": {
                var expectedPlayer = this.playerSetupOrder.shift();
                if (this.currentPlayer.id !== expectedPlayer.id) {
                    throw new Error("Current player is not expected player");
                }

                this.initialPlacementManager.showPlacement(this.currentPlayer, gameEvent.settlementLocation, gameEvent.roadSegmentEndLocation);

                this.currentPlayer.deactivate();

                var nextPlayer = this.playerSetupOrder.length > 0 ? this.playerSetupOrder[0] : null;
                if (nextPlayer) {
                    this.changeCurrentPlayer(nextPlayer.id);
                    nextPlayer.activate();
                }

                break;
            }
            case "StartTurnEvent": {
                this.currentPlayer.deactivate();
                this.changeCurrentPlayer(gameEvent.playerId);
                this.currentPlayer.activate();

                processCollectedResources(this, gameEvent.collectedResources);

                this.diceOne.visible = true;
                this.diceOne.cellIndex = gameEvent.dice1 - 1;

                this.diceTwo.visible = true;
                this.diceTwo.cellIndex = gameEvent.dice2 - 1;

                if (this.currentPlayer.isLocal) {
                    if (this.diceOne + this.diceTwo != 7) {
                        this.build.cellIndex = this.currentPlayer.canBuild() ? BUTTON_NORMAL : BUTTON_DISABLED;
                        this.buy.cellIndex = this.currentPlayer.canBuy() ? BUTTON_NORMAL : BUTTON_DISABLED;
                        this.buildRoad.cellIndex = this.currentPlayer.canBuildRoad() ? BUTTON_NORMAL : BUTTON_DISABLED;
                        this.buildSettlement.cellIndex = this.currentPlayer.canBuildSettlement() ? BUTTON_NORMAL : BUTTON_DISABLED;
                        this.showTurnMenu();
                        this.end.visible = this.end.input.enabled = true;
                    }
                }

                break;
            }
            default: {
                throw new Error(gameEvent.typeName + " not recognised");
            }
        }
    }

    if (!this.playerActions.isEmpty()) {
        var playerAction = this.playerActions.dequeue();

        sendRequest(playerAction, connection);
    }
}