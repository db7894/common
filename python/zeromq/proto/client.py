#! /usr/bin/python
import addressbook_pb2
import sys
import zmq

def get_address_book():
    if len(sys.argv) != 2:
        print "Usage:", sys.argv[0], "ADDRESS_BOOK_FILE"
        sys.exit(-1)
    
    book = addressbook_pb2.AddressBook()
    with open(sys.argv[1], "rb") as f:
        book.ParseFromString(f.read())
    return book

book = get_address_book()
context = zmq.Context()
socket = context.socket(zmq.REQ)
socket.connect("tcp://localhost:5555")
socket.send(book.SerializeToString())
print socket.recv()
