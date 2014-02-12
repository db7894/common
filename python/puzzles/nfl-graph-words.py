#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a graph of connected letters (directed),
starting at any point in the graph attempt to walk
the graph and spell a National Football League team
name.
'''
from common import Graph, Trie

def find_team_names(graph, teams):
    ''' Given a graph of letter connections and a database
    (in this case a trie) of player teams, find which teams
    can be completed using the supplied graph (BFS).

    :param graph: The letter graph to search with
    :param teams: A dictionary of the available teams
    :returns: A generator of the possible solutions
    '''
    queue = graph.get_nodes()
    while queue:
        word = queue.pop()
        if word in teams:
            yield word
        else:
            for letter in graph.get_edges(word[-1]):
                path = word + letter
                if teams.has_path(path):
                    queue.insert(0, path)


def print_team_name(name):
    ''' Given a team name, print it using the
    correct spacing.

    :param name: The name to find the correct spacing of
    '''
    for team in TEAMS:
        if team.replace(' ', '') == name:
            print team
            break
    else: print "no matching team name for: " + team

#------------------------------------------------------------
# constants
#------------------------------------------------------------

TEAMS = [
    'arizona cardinals',
    'atlanta falcons',
    'baltimore ravens',
    'buffalo bills',
    'carolina panthers',
    'chicago bears',
    'cincinnati bengals',
    'cleveland browns',
    'dallas cowboys',
    'denver broncos',
    'detroit lions',
    'green bay packers',
    'houston texans',
    'indianapolis colts',
    'jacksonville jaguars',
    'kansas city chiefs',
    'miami dolphins',
    'minnesotta vikings',
    'new england patriots',
    'new orleans saints',
    'new york jets',
    'oakland raiders',
    'philadelphia eagles',
    'pittsburgh steelers',
    'san diego chargers',
    'san francisco forty niners',
    'seattle seahawks',
    'tampa bay buccaneers',
    'tennessee titans',
    'washington redskins',
]


if __name__ == '__main__':
    teams = Trie()
    teams.add_words(TEAMS)

    graph = Graph(nodes=set('ihgalpsed'))
    graph.add_edges('i', 'h', 'l', 'a')
    graph.add_edges('h', 'i', 'p')
    graph.add_edges('g', 'a', 'l')
    graph.add_edges('a', 'i', 'g', 'l', 'e', 'd')
    graph.add_edges('l', 'i', 'g', 'a', 'e', 'p')
    graph.add_edges('p', 'h', 'l')
    graph.add_edges('s', 'e')
    graph.add_edges('e', 'l', 'a', 's', 'd')
    graph.add_edges('d', 'e', 'a')

    for name in find_team_names(graph, teams):
        print_team_name(name)


