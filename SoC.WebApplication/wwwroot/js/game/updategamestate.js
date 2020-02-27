"use strict"

function updateGameState() {
    Kiwi.State.prototype.update.call(this); // Must use .call otherwise the components property is undefined 

    if (!this.gameEvents.isEmpty()) {
        var gameEvent = this.gameEvents.dequeue();
        switch (gameEvent.typeName) {
            case "ConfirmGameStartEvent": {
                this.initialPlacementManager.showPlacements();

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
                this.initialPlacementManager.activate();
                // TODO: currentPlayer.activate
                break;
            }
            case "ResourcesCollectedEvent": {
                processCollectedResources(this, gameEvent.resourcesCollectedByPlayerId);
                break;
            }
            case "SetupInfrastructurePlacedEvent": {
                this.initialPlacementManager.addPlacement(gameEvent.playerId, gameEvent.settlementLocation, gameEvent.roadSegmentEndLocation);
                break;
            }
            case "StartTurnEvent": {
                processCollectedResources(this, gameEvent.collectedResources);

                this.diceOne.visible = true;
                this.diceOne.cellIndex = gameEvent.dice1 - 1;

                this.diceTwo.visible = true;
                this.diceTwo.cellIndex = gameEvent.dice2 - 1;

                if (this.playersById[gameEvent.playerId].isLocal && this.diceOne + this.diceTwo != 7)
                    this.end.visible = true;

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