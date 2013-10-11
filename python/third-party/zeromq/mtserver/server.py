import time
from threading import Thread, current_thread
import zmq

def worker_thread(url, context):
    socket = context.socket(zmq.REP)
    socket.connect(url)
    info = current_thread()

    while True:
        message = socket.recv()
        print "[%d] Got message %s" % (info.ident, message)
        time.sleep(1)
        socket.send("finished")

def main():
    client_url = "tcp://*:5555"
    worker_url = "inproc://workers"
    context = zmq.Context(1)

    clients = context.socket(zmq.ROUTER)
    clients.bind(client_url)

    workers = context.socket(zmq.DEALER)
    workers.bind(worker_url)

    for _ in range(5):
        thread = Thread(target=worker_thread, args=(worker_url, context))
        thread.start()

    zmq.device(zmq.QUEUE, clients, workers)
    clients.close()
    workers.close()
    context.term()

if __name__ == "__main__":
    main()
