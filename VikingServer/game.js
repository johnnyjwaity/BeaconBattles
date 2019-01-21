class Game {
  constructor(players) {
    for (var i = 0; i < players.length; i++) {
      players[i].game = this;
    }
    this.objectCount = 0;
    this.players = players;
  }
  sendObject(obj, id) {
    // var objID = JSON.parse(obj).id;
    for (var i = 0; i < this.players.length; i++) {
      if (!this.players[i].hasObject(id)) {
        this.players[i].client.sendMessage(obj);
      }
    }
  }
  requestObject() {
    this.objectCount++;
    return this.objectCount;
  }
  startGame() {
    for (var i = 0; i < this.players.length; i++) {
      this.players[i].client.sendMessage(
        JSON.stringify({ type: "game_start" })
      );
    }
  }
}
module.exports = Game;
