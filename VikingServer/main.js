const net = require("net");
const Client = require("./client.js");
const Lobby = require("./lobby.js");
const port = 666;
const server = net.createServer();
server.timeout = 0;
var clients = [];
var lobbys = [];
server.listen(port, "10.0.0.4", () => {
  console.log("Server Running on " + port);
});

server.on("connection", function(socket) {
  var client = new Client(socket);
  clients.push(client);
  client.messageCallback = function(mes) {
    var netData = JSON.parse(mes);
    if (netData.type == "create_lobby") {
      client.name = netData.value;
      var l = new Lobby(client, lobbys.length);
      lobbys.push(l);
    } else if (netData.type == "join_lobby") {
      if (netData.id < lobbys.length) {
        client.name = netData.value;
        lobbys[netData.id].addClient(client);
      }
    }
  };
  // games.push(new Game([new Player(clients[0], "red")]));
  console.log("Connected");
});
