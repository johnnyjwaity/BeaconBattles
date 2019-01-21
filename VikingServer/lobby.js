const Game = require("./game.js");
const Player = require("./player.js");

class Lobby {
  constructor(leader, id) {
    this.clients = [];
    this.id = id;
    this.game = null;
    this.addClient(leader);
    this.clientsReady = 0;
  }
  addClient(client) {
    this.clients.push(client);
    client.sendMessage(
      JSON.stringify({
        type: "lobby_joined",
        id: this.id
      })
    );
    var names = [];
    for (var i = 0; i < this.clients.length; i++) {
      names.push(this.clients[i].name);
    }
    for (var i = 0; i < this.clients.length; i++) {
      var data = {
        type: "update_roster",
        other: names
      };
      this.clients[i].sendMessage(JSON.stringify(data));
    }
    var classRef = this;
    client.messageCallback = function(mes) {
      var data = JSON.parse(mes);
      if (data.type == "request_start") {
        for (var i = 0; i < classRef.clients.length; i++) {
          var data = {
            type: "prepare_game"
          };
          classRef.clients[i].sendMessage(JSON.stringify(data));
        }
      } else if (data.type == "ready_start") {
        classRef.clientsReady += 1;
        if (classRef.clientsReady == classRef.clients.length) {
          var colors = ["red", "green", "yellow", "blue"];
          var players = [];
          for (var c = 0; c < classRef.clients.length; c++) {
            var p = new Player(classRef.clients[c], colors[c]);
            players.push(p);
          }
          var game = new Game(players);
          game.startGame();
          classRef.game = game;
          classRef.clientsReady = 0;
        }
      }
    };
  }
}
module.exports = Lobby;
