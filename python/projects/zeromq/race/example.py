import zmq
import threading

def step1(context):
    print "Finished 1"
    sender = context.socket(zmq.PAIR)
    sender.connect("inproc://step2")
    sender.send("")

def step2(context):
    receiver = context.socket(zmq.PAIR)
    receiver.bind("inproc://step2")

    thread = threading.Thread(target=step1, args=(context,))
    thread.start()

    message = receiver.recv()
    print "Finished 2"

    sender = context.socket(zmq.PAIR)
    sender.connect("inproc://step3")
    sender.send("")


def main():
    context = zmq.Context(1)

    receiver = context.socket(zmq.PAIR)
    receiver.bind("inproc://step3")

    thread = threading.Thread(target=step2, args=(context,))
    thread.start()

    message = receiver.recv()
    print "Finished 3"

    receiver.close()
    context.term()

if __name__ == "__main__":
    main()
