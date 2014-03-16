from itertools import islice, count
from math import factorial as fact

#------------------------------------------------------------
# Simple Sequences
#------------------------------------------------------------
square_sequence       = lambda n, p, c: n**2
cube_sequence         = lambda n, p, c: n**3
fibonacci_sequence    = lambda n, p, c: c + p
triangle_sequence     = lambda n, p, c: (n * (n + 1)) / 2
pentagon_sequence     = lambda n, p, c: ((3 * n**2) - n) / 2
hexagon_sequence      = lambda n, p, c: ((2 * n) * ((2 * n) - 1)) / 2
lazy_caterer_sequence = lambda n, p, c: (n**2 + n + 2) / 2
magic_number_sequence = lambda n, p, c: (n * (n**2 + 1)) / 2
catalan_sequence      = lambda n, p, c: fact(2 * n)  / (fact(n + 1) * fact(n))

#------------------------------------------------------------
# Complex Sequences
#------------------------------------------------------------
def look_and_say_sequence(num, prev, curr):
    '''
    :param num: The element number to generate
    :param prev: The previous entry into the sequence
    :param curr: The current entry into the sequence
    :returns: The next element of the sequence
    '''
    string = str(curr) + ' '
    c, n, r = string[0], 0, []
    for char in string:
        if char != c:
            r.extend([str(n), c]) 
            n, c = 1, char
        else: c, n = char, n + 1
    return int(''.join(r))

def lucas_sequence(num, prev=2, curr=1):
    '''
    :param num: The element number to generate
    :param prev: The previous entry into the sequence
    :param curr: The current entry into the sequence
    :returns: The next element of the sequence
    '''
    return prev + curr

def sequence_generator(sequence, prev=0, curr=1):
    ''' Given a sequence and its previous and current
    elements, will generate an infinite stream of elements
    of the sequence.

    :param sequence: The sequence to generate
    :param prev: The previous entry into the sequence
    :param curr: The current entry into the sequence
    :returns: A generator of the sequence's elements
    '''
    yield 0, prev
    for n in count(1):
        prev, curr = curr, sequence(n, prev, curr)
        yield n, curr

def take_n_of_sequence(sequence, n=10):
    ''' Given a sequence, take the first N values of
    said sequence.

    :param sequence: The sequence to print values from
    :param n: The number of elements to print of the sequence
    '''
    return [v for _, v in islice(sequence, 0, n)]

def print_n_of_sequence(sequence, n=10):
    ''' Given a sequence, print the first N values of
    said sequence.

    :param sequence: The sequence to print values from
    :param n: The number of elements to print of the sequence
    '''
    generator = sequence_generator(sequence)
    for num, elem in islice(generator, 0, n):
        print "%2d: %d" % (num, elem)

if __name__ == "__main__":
    print_n_of_sequence(fibonacci_sequence)
