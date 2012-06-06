import zmq
from random import randrange

context = zmq.Context()
socket = context.socket(zmq.PUB)
socket.bind("tcp://*:5556")
socket.bind("ipc://weather.ipc")

while True:
    zipcode     = randrange(1,1000000)
    temperature = randrange(1, 215) - 80
    humidity    = randrange(1, 50) + 10
    message = "%d %d %d" % (zipcode, temperature, humidity)
    socket.send(message)

socket.close()
context.term()
