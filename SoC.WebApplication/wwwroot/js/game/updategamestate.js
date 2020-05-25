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
                    var i = 1 / 0; //TODO throw better exception
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
                processCollectedResources(this, gameEvent.collectedResources);

                this.diceOne.visible = true;
                this.diceOne.cellIndex = gameEvent.dice1 - 1;

                this.diceTwo.visible = true;
                this.diceTwo.cellIndex = gameEvent.dice2 - 1;

                if (this.playersById[gameEvent.playerId].isLocal) {
                    if (this.diceOne + this.diceTwo != 7) {
                        this.build.visible = this.build.input.enabled = true;
                        this.build.cellIndex = this.currentPlayer.canBuild() ? BUTTON_NORMAL : BUTTON_DISABLED;
                        this.buildSettlement.cellIndex = this.player.canBuildSettlement() ? BUTTON_NORMAL : BUTTON_DISABLED;
                        this.end.visible = true;
                    }
                }

                break;
            }
            default: {
                break;
            }
        }
    }

    if (!this.playerActions.isEmpty()) {
        var playerAction = this.playerActions.dequeue();

        sendRequest(playerAction, connection);
    }
}