#!/usr/bin/env python
# -*- coding: latin-1 -*-
import sys

def chain(*streams):
    ''' Given a collection of streams, yield
    from each in order until they are all exhausted.

    :param streams: The streams to yield from
    :returns: A generator around the streams
    '''
    for stream in streams:
        for char in stream:
            yield char

def charcter_generator(path):
    ''' Given a file handle, convert the file
    to a character stream.

    :param path: The path to the file
    :returns: A character stream around the file
    '''
    with open(path) as handle:
        for line in handle:
            for char in line:
                yield char

class Token(object):
    ''' A collection of the tokens we are matching
    against (just to keep them all in one place).
    '''
    NewLine = '\n'
    Slash   = '/'
    Hash    = '#'
    Quote   = '"'
    Quote2  = "'"
    Star    = '*'

class CommentParser(object):
    ''' A simple recursive descent parser to handle
    printing out the comments embedded in a source
    code file.
    '''

    def __init__(self, stream, output):
        ''' Initialize a new instance of the CommentParser

        :param stream: The stream to parse
        :param output: The output handle to write to
        '''
        self.stream = stream
        self.output = output

    def handle_string(self, match):
        ''' Handle the state where we are in a string
        literal between the following::
            
            " ... "
        '''
        while self.stream.next() != match:
            pass
    
    def handle_line_comment(self):
        ''' Handle the state where we are in a line comment
        between the following::
            
            // ... \n
        '''
        curr = self.stream.next()
        while curr != Token.NewLine:
            self.output.write(curr)
            curr = self.stream.next()
        self.output.write(curr)
    
    def handle_block_comment(self):
        ''' Handle the state where we are in a block comment
        between the following::
            
            /* ... */
        '''
        prev, curr = self.stream.next(), self.stream.next()
        while prev != Token.Star and curr != Token.Slash:
            self.output.write(prev)
            prev, curr = curr, self.stream.next()

    def handle_triple_comment(self, match):
        ''' Handle the state where we are in a block comment
        between the following::
            
            """ ... """

        :param match: The quote match to operate with
        '''
        left, prev, curr = self.stream.next(), self.stream.next(), self.stream.next()
        while prev != match and curr != match and left != match:
            self.output.write(left)
            left, prev, curr = prev, curr, self.stream.next()

    def handle_quote(self, match):
        ''' Handle the state where we are either in a quote
        or a triple quote comment.

        :param match: The quote match to operate with
        '''
        prev = self.stream.next()
        if prev == match:
            curr = self.stream.next()
            if curr == match: self.handle_triple_comment(match)
            else: self.stream = chain([curr], self.stream)
        else: self.handle_string(match)
    
    def parse(self):
        ''' Begin parsing the stream of data until
        the stream is exhausted.
        '''
        try:
            while True:
                char = self.stream.next()
                if   char == Token.Quote:  self.handle_quote(char)
                elif char == Token.Quote2: self.handle_quote(char)
                elif char == Token.Hash:   self.handle_line_comment()
                elif char == Token.Slash:
                    char = self.stream.next()
                    if   char == Token.Slash: self.handle_line_comment()
                    elif char == Token.Star:  self.handle_block_comment()
                    else: self.stream = chain([char], self.stream)
        except StopIteration: pass # we are done with the stream

if __name__ == "__main__":
    stream = charcter_generator(sys.argv[1])
    output = sys.stdout
    CommentParser(stream, output).parse()
