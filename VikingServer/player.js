class Player {
  constructor(client, color) {
    this.client = client;
    this.color = color;
    var ref = this;
    this.client.messageCallback = function(message) {
      ref.interpretMessage(message);
    };
    this.game = null;
    this.objects = [];
  }
  interpretMessage(mes) {
    // this.opp.client.sendMessage(mes);
    var netData = JSON.parse(mes);
    if (netData.type == "object") {
      this.game.sendObject(mes, netData.id);
      // this.opp.client.sendMessage(mes);
    } else if (netData.type == "request_object") {
      if (this.game != null) {
        var objId = this.game.requestObject();
        this.objects.push(objId);
        console.log("Creating Object");
        var sendData = {
          type: "object_id",
          id: objId
        };
        this.client.sendMessage(JSON.stringify(sendData));
      }
    } else if (netData.type == "animation") {
      this.game.sendObject(mes, netData.id);
    } else if (netData.type == "damage") {
      for (var i = 0; i < this.game.players.length; i++) {
        var p = this.game.players[i];
        if (p.hasObject(netData.id)) {
          var obj = JSON.stringify({
            type: "recieve_damage",
            key: netData.key,
            id: netData.id,
            value: netData.value
          });
          p.client.sendMessage(obj);
          console.log(obj);
          break;
        }
      }
    }
  }
  hasObject(obj) {
    for (var i = 0; i < this.objects.length; i++) {
      if (this.objects[i] == obj) {
        return true;
      }
    }
    return false;
  }
}
module.exports = Player;
