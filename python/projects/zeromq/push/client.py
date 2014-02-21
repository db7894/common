import sys
import zmq

context = zmq.Context()
socket  = context.socket(zmq.SUB)
socket.connect("tcp://localhost:5556")
socket.setsockopt(zmq.SUBSCRIBE, "10001")

for update in range(10):
    message = socket.recv()
    z,t,h   = message.split()
    print "zip[%s] temp[%s] hum[%s]" % (z,t,h)
