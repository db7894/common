import zmq

context = zmq.Context()
print "Connecting to server..."

socket = context.socket(zmq.REQ)
socket.connect("tcp://localhost:5555")

for request in xrange(10000):
    print "Sending command %d..." % request,
    socket.send("%d + %d" % (request, request))

    message = socket.recv()
    print "%d [%s]" % (request, message)
