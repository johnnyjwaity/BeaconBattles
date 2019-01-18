class Player {
  constructor(client, color) {
    this.client = client;
    this.color = color;
    var ref = this;
    this.client.messageCallback = function(message) {
      ref.interpretMessage(message);
    };
    this.game = null;
  }
  interpretMessage(mes) {
    var netData = JSON.parse(mes);
    if (netData.type == "object") {
      this.client.sendMessage(mes);
    } else if (netData.type == "request_object") {
      if (this.game != null) {
        var objId = this.game.requestObject();
        console.log("Creating Object");
        var sendData = {
          type: "object_id",
          key: "",
          value: "",
          id: objId
        };
        this.client.sendMessage(JSON.stringify(sendData));
      }
    }
  }
}
module.exports = Player;
