class Game {
  constructor(players) {
    for (var i = 0; i < players.length; i++) {
      players[i].game = this;
    }
//initializes health of players and beacons
    this.objectCount = 0;
    this.players = players;
    this.beaconHealth = [100, 100, 100, 100];
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
    //takes into account what beacon is being attacked from the color parameter and damages beacon accordingly
  damageBeacon(color, amount) {
    var index = 0;
    if (color == "red") {
      index = 0;
    } else if (color == "green") {
      index = 1;
    } else if (color == "yellow") {
      index = 2;
    } else if (color == "blue") {
      index = 3;
    }
    console.log("Damaging Beacon " + color + "by " + amount);
    this.beaconHealth[index] -= amount;
    if (this.beaconHealth[index] <= 0) {
      var netData = {
        type: "destroy_beacon",
        id: index,
        value: color
      };
      this.sendObject(JSON.stringify(netData), -1);
    }
  }
    //takes players back into the lobby
  endGame() {
    this.lobby.endGame();
  }
}
module.exports = Game;
