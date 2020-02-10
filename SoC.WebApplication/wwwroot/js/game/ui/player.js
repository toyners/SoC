
class Player {
    constructor(state, name, x, y, topDown) {
        this.settlementCount = 5;
        this.cityCount = 4;
        this.roadCount = 15;

        var diffY = topDown ? 40 : -40;

        var playerName = new Kiwi.GameObjects.Textfield(state, name, x, y, "#000", 32, 'normal', 'Impact');
        state.addChild(playerName);

        y += diffY;

        this.settlementCounter = new Kiwi.GameObjects.Textfield(state, this.settlementCount + 'x', x, y, "#000", 22, 'normal', 'Impact');
        state.addChild(this.settlementCounter);
        
        //var settlementIcon = new Kiwi.GameObjects.StaticImage(this, this.textures.background, 0, 0);
    }
}