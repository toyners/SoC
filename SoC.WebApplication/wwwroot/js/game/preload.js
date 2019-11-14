"use strict";

function preload() {
    Kiwi.State.prototype.preload(this);
    this.addImage('background', '../../../images/background.png');
    this.addImage('brickhex', '../../../images/hextypes/brick.png');
    this.addImage('deserthex', '../../../images/hextypes/desert.png');
    this.addImage('grainhex', '../../../images/hextypes/grain.png');
    this.addImage('lumberhex', '../../../images/hextypes/lumber.png');
    this.addImage('orehex', '../../../images/hextypes/ore.png');
    this.addImage('woolhex', '../../../images/hextypes/wool.png');
    this.addImage('two', '../../../images/productionfactors/2.png');
    this.addImage('three', '../../../images/productionfactors/3.png');
    this.addImage('four', '../../../images/productionfactors/4.png');
    this.addImage('five', '../../../images/productionfactors/5.png');
    this.addImage('six', '../../../images/productionfactors/6.png');
    this.addImage('eight', '../../../images/productionfactors/8.png');
    this.addImage('nine', '../../../images/productionfactors/9.png');
    this.addImage('ten', '../../../images/productionfactors/10.png');
    this.addImage('eleven', '../../../images/productionfactors/11.png');
    this.addImage('twelve', '../../../images/productionfactors/12.png');
    this.addImage('redsettlement', '../../../images/settlement/redsettlement.png');
    this.addImage('redsettlementhover', '../../../images/settlement/redsettlementhover.png');
    this.addImage('settlementicon', '../../../images/settlement/settlementicon.png');
    this.addImage('roadhorizontalicon', '../../../images/roadhorizontalicon.png')
    this.addImage('roadhorizontaliconhover', '../../../images/roadhorizontalicon_redhover.png')

    this.addSpriteSheet('playermarker', '../../../images/currentplayermarker.png', 50, 45);
}