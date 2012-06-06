import time
import zmq

def main():
    context = zmq.Context(1)
    publisher = context.socket(zmq.PUB)
    publisher.bind("tcp://*:5563")

    while True:
        publisher.send_multipart(["A", "This shouldn't be seen"])
        publisher.send_multipart(["B", "This should be seen"])
        time.sleep(1)
    publisher.close()
    context.term()

if __name__ == "__main__":
    main()
