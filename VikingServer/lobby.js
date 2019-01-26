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
  endGame() {
    for (var i = 0; i < this.clients.length; i++) {
      this.setClientCallback(this.clients[i]);
    }
    this.game = null;
  }
  addClient(client) {
    this.clients.push(client);
    client.sendMessage(
      JSON.stringify({
        type: "lobby_joined",
        id: this.id
      })
    );
    this.sendRoster();
    this.setClientCallback(client);
    var classRef = this;
    client.onDisconnect = function() {
      console.log("Disconnecting");
      classRef.clients.splice(classRef.clients.indexOf(client), 1);
      classRef.sendRoster();
    };
  }
  sendRoster() {
    console.log("sending Roster");
    console.log(this.clients.length);
    var names = [];
    for (var i = 0; i < this.clients.length; i++) {
      names.push(this.clients[i].name);
    }
    for (var i = 0; i < this.clients.length; i++) {
      var data = {
        type: "update_roster",
        other: names
      };
      console.log(data);
      this.clients[i].sendMessage(JSON.stringify(data));
    }
  }
  setClientCallback(client) {
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
          game.lobby = classRef;
          classRef.game = game;
          classRef.clientsReady = 0;
        }
      }
    };
  }
}
module.exports = Lobby;
