import zmq

def main():
    context = zmq.Context(1)
    subscriber = context.socket(zmq.SUB)
    subscriber.connect("tcp://localhost:5563")
    subscriber.setsockopt(zmq.SUBSCRIBE, "B")

    while True:
        (address, contents) = subscriber.recv_multipart()
        print contents
    subscriber.close()
    context.term()

if __name__ == "__main__":
    main()

