"use strict"

function updateGameState() {
    Kiwi.State.prototype.update.call(this); // Must use .call otherwise the components property is undefined 

    if (!this.gameEvents.isEmpty()) {
        var gameEvent = this.gameEvents.dequeue();
        switch (gameEvent.typeName) {
            case "ConfirmGameStartEvent": {
                this.initialPlacementManager.showPlacements();

                var request = {
                    gameId: gameId,
                    playerId: this.playerId,
                    playerActionType: 'ConfirmGameStartAction',
                    data: JSON.stringify({
                        initiatingPlayerId: this.playerId
                    })
                };
                connection.invoke("PlayerAction", request).catch(function (err) {
                    return console.error(err.toString());
                });
                break;
            }
            case "PlaceSetupInfrastructureEvent": {
                this.initialPlacementManager.activate();
                this.currentPlayerMarker.visible = true;
                this.currentPlayerMarker.animation.play('main');
                break;
            }
            case "ResourcesCollectedEvent": {
                for (var playerId in gameEvent.resourcesCollectedByPlayerId) {
                    var player = this.playersById[playerId];
                    var resourcesForPlayer = gameEvent.resourcesCollectedByPlayerId[playerId];
                    if (resourcesForPlayer) {
                        for (var resourcesForLocation in resourcesForPlayer) {
                            var resources = resourcesForLocation.resources;
                            if (resources.brickCount)
                                player.updateBrickCount(resources.brickCount);
                        }
                    }
                }

                break;
            }
            case "SetupInfrastructurePlacedEvent": {
                this.initialPlacementManager.addPlacement(gameEvent.playerId, gameEvent.settlementLocation, gameEvent.roadSegmentEndLocation);
                break;
            }
            case "StartTurnEvent": {
                break;
            }
            default: {
                break;
            }
        }
    }

    if (this.initialPlacementManager.isConfirmed()) {
        this.currentPlayerMarker.visible = false;
        var placementData = this.initialPlacementManager.getData();
        if (placementData) {
            var request = {
                gameId: gameId,
                playerId: this.playerId,
                playerActionType: 'PlaceSetupInfrastructureAction',
                data: JSON.stringify({
                    initiatingPlayerId: this.playerId,
                    settlementLocation: placementData.settlementLocation,
                    roadEndLocation: placementData.roadEndLocation
                })
            };

            connection.invoke("PlayerAction", request).catch(function (err) {
                return console.error(err.toString());
            });
        }
    }
}