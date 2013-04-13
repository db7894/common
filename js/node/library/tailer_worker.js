var fs = require('fs');

/**
 * Given a socket and a file, tail the file
 * into the socket.
 *
 * @param socket The socket to write to
 * @param file The file to tail from
 */
process.on('socket', function(socket, file) {

  // helper to write sse events
  var id = (new Date()).toLocaleTimeString();
  function write_message(data) {
    socket.write('id: '   + id + '\n');
    socket.write('data: ' + data + '\n\n');
  };

  // read the file initially
  fs.readFile(file, function(error, initial) {
    if (error) throw error;
    write_message(initial);

    // write file updates as they come
    fs.watchFile(file, function(curr, prev) {
      console.log("new filesystem change %s", curr);
      if (curr.size > prev.size) {
        var stream = fs.createReadStream(file, {
          start: prev.size,
          end: curr.size - 1,
          encoding: 'utf-8'
        });
        stream.on('data', write_message);
        stream.on('error', function(error) {
          console.log('watch error: %s', error);
        });
      }
    });

  });
});
