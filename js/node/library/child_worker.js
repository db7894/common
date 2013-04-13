var exec = require('child_process').exec;

process.on('socket', function(socket) {
  socket.on('command', function(command) {
    console.log("executing command %s", command);
    exec(command, function(stdout, stderr, error) {
      socket.emit('results', {
        'output': stdout,
        'errors': error,
      });
    });
  });
});
