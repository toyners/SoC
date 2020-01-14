"use strict";

function preload() {
    Kiwi.State.prototype.preload(this);
    this.addImage('background', '../../../images/background.png');
    this.addSpriteSheet('hextypes', '../../../images/hextypes.png', 100, 100);
    this.addSpriteSheet('productionfactors', '../../../images/productionfactors.png', 100, 100);

    this.addSpriteSheet('settlement', '../../../images/settlement/settlement.png', 25, 25);
    this.addSpriteSheet('angular_roads', '../../../images/road/angular_roads.png', 26, 41);
    this.addSpriteSheet('horizontal_roads', '../../../images/road/horizontal_roads.png', 37, 10);
    this.addSpriteSheet('playermarker', '../../../images/currentplayermarker.png', 50, 45);
    this.addSpriteSheet('confirm', '../../../images/confirm.png', 30, 30);
    this.addSpriteSheet('cancel', '../../../images/cancel.png', 30, 30);

    //this.addImage('test', '../../../images/test.png');
    this.addImage('test', '../../../images/100x100 test.png')
}