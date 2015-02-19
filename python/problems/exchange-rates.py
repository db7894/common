#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a graph of connected letters (directed),
starting at any point in the graph attempt to walk
the graph and spell a National Football League team
name.
'''

def find_arbitrages(exchanges):
    ''' Given a graph of exchanges and the exchange rates,
    attempt to find an arbitrage where we can make money
    just by trading currencies.

    :param exchanges: The exchange graph
    :returns: A generator of the available arbitrages
    '''
    queue = [(1.0, ['U'])]
    while queue:
        rate, path = queue.pop()
        if path[-1] == path[0] and rate > 1.0:
            yield rate, path
        else:
            for name, exch in exchanges[path[-1]].items():
                queue.insert(0, (rate * exch, path + [name]))

def print_arbitrage(exchanges, arbitrage):
    ''' Given a team name, print it using the
    correct spacing.

    :param name: The name to find the correct spacing of
    '''
    rate = 1.000
    prev = arbitrage.pop(0)
    path = [(rate, prev)]
    for curr in arbitrage:
        prev, rate = curr, rate * exchanges[prev][curr]
        path.append((prev, rate))
    print ' → '.join("{}:{}".format(exch, rate) for exch, rate in path)

#------------------------------------------------------------
# constants
#------------------------------------------------------------

EXCHANGES = {
    'U': { 'U':1.0000, 'E':0.8148, 'G':0.6404, 'J':78.125, 'F':0.9784, 'C':0.9924, 'A':0.9465 },
    'E': { 'U':1.2275, 'E':1.0000, 'G':0.7860, 'J':96.550, 'F':1.2010, 'C':1.2182, 'A':1.1616 },
    'G': { 'U':1.5617, 'E':1.2724, 'G':1.0000, 'J':122.83, 'F':1.5280, 'C':1.5498, 'A':1.4778 },
    'J': { 'U':0.0128, 'E':0.0104, 'G':0.0081, 'J':1.0000, 'F':1.2442, 'C':0.0126, 'A':0.0120 },
    'F': { 'U':1.0219, 'E':0.8327, 'G':0.6546, 'J':80.390, 'F':1.0000, 'C':1.0142, 'A':0.9672 },
    'C': { 'U':1.0076, 'E':0.8206, 'G':0.6453, 'J':79.260, 'F':0.9859, 'C':1.0000, 'A':0.9535 },
    'A': { 'U':1.0567, 'E':0.8609, 'G':0.6767, 'J':83.130, 'F':1.0339, 'C':1.0487, 'A':1.0000 },
}

if __name__ == '__main__':
    for rate, arbitrage in find_arbitrages(EXCHANGES):
        print_arbitrage(EXCHANGES, arbitrage)
