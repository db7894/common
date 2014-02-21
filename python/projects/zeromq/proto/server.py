#! /usr/bin/python

# See README.txt for information and build instructions.

import addressbook_pb2
import sys
import zmq

def ListPeople(address_book):
    for person in address_book.person:
        print "Person ID:", person.id
        print "  Name:", person.name
        if person.HasField('email'):
            print "  E-mail address:", person.email
        
        for phone_number in person.phone:
            if phone_number.type == addressbook_pb2.Person.MOBILE:
                print "  Mobile phone #:",
            elif phone_number.type == addressbook_pb2.Person.HOME:
                print "  Home phone #:",
            elif phone_number.type == addressbook_pb2.Person.WORK:
                print "  Work phone #:",
            print phone_number.number

context = zmq.Context()
socket  = context.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5555")
book = addressbook_pb2.AddressBook()
while True:
    message = socket.recv()
    book.ParseFromString(message)
    print book#ListPeople(book)
    print book.ListFields()
    socket.send("ok")
