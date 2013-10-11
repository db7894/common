import zmq

print "Starting server..."
context = zmq.Context()
socket  = context.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5555")

while True:
    message = socket.recv()
    socket.send("result %d" % eval(message))
