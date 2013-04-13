var http    = require('http'),
    sys     = require('sys'),
    fs      = require('fs'),
    spawn   = require('child_process').spawn,
    port    = 8080,
    delay   = 5080,
    command = 'w';

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
      sendSSE(req, res);
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
 * @param req The initial sse reqeust
 * @param res The resource to write to
 */
function sendSSE(req, res) {
  res.writeHead(200, {
    'Content-Type':  'text/event-stream',
    'Cache-Control': 'no-cache',
    'Connection':    'keep-alive'
  });

  var id = (new Date()).toLocaleTimeString();

  setInterval(function() {
    spawn(command).stdout.on('data', function(data) {
      constructSSE(res, id, data);
    });
  }, delay);

  var data = (new Date()).toLocaleTimeString() + ": Starting system monitor";
  constructSSE(res, id, data);
  //res.end();
}

/**
 * A helper method to construct an SSE message
 *
 * @param res The resource to write to
 * @param id The id of the event to send
 * @param data The data to send via SSE
 */
function constructSSE(res, id, data) {
  res.write('id: ' + id + '\n');
  res.write("data: " + data + '\n\n');
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
