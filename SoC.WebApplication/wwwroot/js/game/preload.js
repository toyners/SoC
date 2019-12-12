"use strict";

function preload() {
    Kiwi.State.prototype.preload(this);
    this.addImage('background', '../../../images/background.png');
    this.addSpriteSheet('hextypes', '../../../images/hextypes.png', 100, 100);
    this.addSpriteSheet('productionfactors', '../../../images/productionfactors.png', 100, 100);

    this.addImage('roadhorizontaliconhover', '../../../images/road/roadhorizontalicon_redhover.png');
    this.addImage('roadupperlefticon', '../../../images/road/roadupperlefticon.png');
    this.addSpriteSheet('settlement', '../../../images/settlement/settlement.png', 25, 25);
    this.addSpriteSheet('road', '../../../images/road/road.png', 28, 11);
    this.addSpriteSheet('playermarker', '../../../images/currentplayermarker.png', 50, 45);
    this.addSpriteSheet('confirm', '../../../images/confirm.png', 30, 30);
    this.addSpriteSheet('cancel', '../../../images/cancel.png', 30, 30);

    //this.addImage('test', '../../../images/test.png');
    this.addImage('test', '../../../images/100x100 test.png')
}