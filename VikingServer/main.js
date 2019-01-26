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
    if (netData.value.charAt(netData.value.length - 1) == "?") {
      netData.value = netData.value.slice(0, -1);
    }
      if (netData.type == "create_lobby") {
          //creates a new lobby when create lobby button is pressed
      console.log("Creating Lobby");
      console.log(mes);
      console.log(netData);
      client.name = netData.value;
      var l = new Lobby(client, lobbys.length);
      lobbys.push(l);
    } else if (netData.type == "join_lobby") {
        if (netData.id < lobbys.length) {
            //successfully joined lobby
        client.name = netData.value;
        lobbys[netData.id].addClient(client);
      } else {
          //when and incorrect lobby code is entered in and the lobby doesnt exist
        client.sendMessage(
          JSON.stringify({
            type: "join_failed",
            key: "Lobby Does Not Exist"
          })
        );
      }
    }
  };
  // games.push(new Game([new Player(clients[0], "red")]));
  console.log("Connected");
});
