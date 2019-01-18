class Game {
  constructor(players) {
    for (var i = 0; i < players.length; i++) {
      players[i].game = this;
    }
    this.objectCount = 0;
    this.players = players;
  }
  requestObject() {
    this.objectCount++;
    return this.objectCount;
  }
}
module.exports = Game;
