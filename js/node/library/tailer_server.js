var http    = require('http'),
    sys     = require('sys'),
    fs      = require('fs'),
    fork    = require('child_process').fork,
    worker  = 'tailer_worker.js',
    port    = 8080;

/**
 * Create an HTTP SSE server
 *
 * @param req The http request to handle
 * @param res The resource to write to
 */
http.createServer(function(req, res) {
  debugHeaders(req);

  if (req.headers.accept && req.headers.accept == 'text/event-stream') {
    if (req.url == '/events') {
      startServerSideEvents(req, res);
    } else {
      res.writeHead(404);
      res.end();
    }
  } else {
    res.writeHead(200, {'Content-Type': 'text/html'});
    res.write(fs.readFileSync(__dirname + '/server_side_events.html'));
    res.end();
  }
}).listen(port);

/**
 * Send a server side event to the specified user
 *
 * @param request The initial sse reqeust
 * @param socket The socket to write to
 */
function startServerSideEvents(request, socket) {
  socket.writeHead(200, {
    'Content-Type':  'text/event-stream',
    'Cache-Control': 'no-cache',
    'Connection':    'keep-alive'
  });

  var child = fork(worker);
  child.send('socket', res);
}

/**
 * A helper method to dumpt the request headers
 *
 * @param req The request to dump the headers of
 */
function debugHeaders(req) {
  sys.puts('URL: ' + req.url);
  for (var key in req.headers) {
    sys.puts(key + ': ' + req.headers[key]);
  }
  sys.puts('\n\n');
}
