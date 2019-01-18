const net = require("net");
const Client = require("./client.js");
const Player = require("./player.js");
const Game = require("./game.js");
const port = 666;
const server = net.createServer();
var clients = [];
var games = [];
server.listen(port, "10.0.0.4", () => {
  console.log("Server Running on " + port);
});

server.on("connection", function(socket) {
  clients.push(new Client(socket));
  games.push(new Game([new Player(clients[0], "red")]));
  console.log("Connected");
});
