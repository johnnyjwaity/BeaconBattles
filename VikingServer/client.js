const spawn = require("threads").spawn;
class Client {
  constructor(socket) {
    socket.isReading = false;
    socket.messageLength = 0;
    socket.readBytes = null;
    socket.unaccountedBytes = [];
    this.onDisconnect = null;
    var classRef = this;
    socket.on("error", function(err) {
      // console.log(err);
    });
    socket.on("close", function(data) {
      console.log("Disconnected");
      if (classRef.onDisconnect != null) {
        classRef.onDisconnect();
      }
    });
    var classRef = this;
    socket.on("data", function(data) {
      // console.log("Recieved Data of Length: " + data.length);
      socket.unaccountedBytes.push(...data);
      // classRef.readMessage();
      if (!socket.isReading) {
      }
    });
    setInterval(function() {
      classRef.readMessage();
    }, 1.0 / 60.0);
    this.messageCallback = null;
    this.socket = socket;
  }
    sendMessage(msg) {
        //sends a message to server for the server to interpret and sync to all clients
    var buffer = Buffer.from(msg, "ascii");
    var length = buffer.length;
    var strLength = length.toString(16);
    if (strLength.length > 4) {
      console.log("Message Exceeds 4 byte limit");
      socket.end();
      return;
    }
    while (strLength.length < 4) {
      strLength = "0" + strLength;
    }
    var totalMes = strLength + buffer.toString("ascii");
    this.socket.write(totalMes, "ascii");
  }
    readMessage() {
        //interprets messages received from server
    if (this.socket.unaccountedBytes.length == 0) {
      return;
    }
    //   console.log("Bytes Remaining: " + socket.unaccountedBytes.length);
    if (!this.socket.isReading) {
      this.socket.isReading = true;
      this.socket.readBytes = [];
      this.socket.messageLength = parseInt(
        Buffer.from([
          this.socket.unaccountedBytes.shift(),
          this.socket.unaccountedBytes.shift(),
          this.socket.unaccountedBytes.shift(),
          this.socket.unaccountedBytes.shift()
        ]).toString("ascii"),
        16
      );
      // console.log("New Message of Length " + socket.messageLength);
      // console.log("Remaining Bytes: " + socket.unaccountedBytes.length);
    }
    while (this.socket.unaccountedBytes.length > 0) {
      if (this.socket.readBytes.length < this.socket.messageLength) {
        this.socket.readBytes.push(this.socket.unaccountedBytes.shift());
        //   console.log(
        //     "Added Byte. Current Read Length: " + socket.readBytes.length
        //   );
      }
      if (this.socket.readBytes.length == this.socket.messageLength) {
        //   console.log(
        //     "Message Completed " + Buffer.from(socket.readBytes).toString("ascii")
        //   );
        if (this.messageCallback != null) {
          this.messageCallback(
            Buffer.from(this.socket.readBytes).toString("ascii")
          );
        }
        // this.sendMessage(Buffer.from(this.socket.readBytes).toString("ascii"));
        this.socket.isReading = false;
        break;
      }
    }
    if (this.socket.unaccountedBytes.length != 0) {
      this.readMessage();
    }
  }
  disconnect() {}
}
module.exports = Client;
