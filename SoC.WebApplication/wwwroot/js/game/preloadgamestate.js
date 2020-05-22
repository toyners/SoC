"use strict";

function preloadGameState() {
    Kiwi.State.prototype.preload(this);

    var controlImagePath = '../../../images/control/';

    this.addImage('background', '../../../images/background.png');
    this.addSpriteSheet('hextypes', '../../../images/hextypes.png', 100, 100);
    this.addSpriteSheet('productionfactors', '../../../images/productionfactors.png', 100, 100);

    this.addSpriteSheet('settlement', '../../../images/settlement/settlement.png', 25, 25);
    this.addSpriteSheet('angularRoads', '../../../images/road/angular_roads.png', 26, 41);
    this.addSpriteSheet('horizontalRoads', '../../../images/road/horizontal_roads.png', 37, 10);
    this.addSpriteSheet('marker', '../../../images/player/playermarker.png', 50, 45);
    this.addSpriteSheet('reverseMarker', '../../../images/player/reverseplayermarker.png', 50, 45);

    this.addSpriteSheet('back', controlImagePath + 'back.png', 70, 30)
    this.addSpriteSheet('build', controlImagePath + 'build.png', 70, 30)
    this.addSpriteSheet('cancel', controlImagePath + 'cancel.png', 30, 30);
    this.addSpriteSheet('confirm', controlImagePath + 'confirm.png', 96, 30);
    this.addSpriteSheet(buttonSettlementImageName, controlImagePath + 'settlement.png', 121, 30);
    this.addSpriteSheet('end', controlImagePath + 'end.png', 107, 30)

    this.addSpriteSheet('resourceTypes', '../../../images/player/resourceTypes.png', 20, 20)
    this.addSpriteSheet('settlementIcons', '../../../images/player/settlementIcons.png', 20, 20)
    this.addSpriteSheet('roadIcons', '../../../images/player/roadIcons.png', 20, 20)

    this.addSpriteSheet('dice', '../../../images/dice.png', 40, 40)

    //this.addImage('test', '../../../images/test.png');
    this.addImage('test', '../../../images/100x100 test.png')
}