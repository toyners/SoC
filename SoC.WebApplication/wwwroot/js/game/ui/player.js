
class Player {
    constructor(state, name, x, y, topDown) {
        this.settlementCount = 15;
        this.roadCount = 5;
        this.cityCount = 4;

        var diffY = topDown ? 40 : -40;

        var playerName = new Kiwi.GameObjects.Textfield(state, name, x, y, "#000", 32, 'normal', 'Impact');
        state.addChild(playerName);

        y += diffY;

        this.settlementCounter = new Kiwi.GameObjects.Textfield(state, String.toString(this.settlementCount) + 'x', x, y, "#000", 22, 'normal', 'Impact');
        state.addChild(this.settlementCounter);
    }

    addSettlement(texture, state) {
        var settlement = new Kiwi.GameObjects.Sprite(state, texture, x, y);
        state.addChild(settlement);
    }
}