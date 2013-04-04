var worker = require('child_process').fork('child_worker.js'),
    io     = require('socket.io').listen(12345);

io.sockets.on('connection', function(socket) {
  worker.send('socket', socket);
});
