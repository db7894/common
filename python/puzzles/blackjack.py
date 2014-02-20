#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given five cards, insert basic operations between them
such that the total is 21. The rules:

* all non-face cards are their value 
* all face cards are 10
* all aces are 1 or 11
* the evaluation happens from left to right
* the operations are {+ - / *}
* the current total cannot be negative
* the current total must be a whole number
'''
import operator as O

#------------------------------------------------------------
# constants
#------------------------------------------------------------

OPERATIONS  = [O.add, O.sub, O.div, O.mul]
OP_STRINGS  = {O.add: '+', O.sub: '-', O.div: '÷', O.mul: '×'}
CARD_VALUES = { chr(48 + x):x for x in range(2, 10) }
CARD_VALUES.update([('J', 10), ('Q', 10), ('K', 10), ('A', [1, 11])])

#------------------------------------------------------------
# utilities
#------------------------------------------------------------

def evaluate(hand, total=21):
    ''' Given a full hand of cards and the operations between
    the cards, return wether the final total is equal to the
    specified total amount.

    :param hand: The hand to evalute the result for
    :param total: The total the hand should evaluate to (default 21)
    :returns: True if the result equals the supplied total, False otherwise

    >>> hand = [11, O.sub, 2, O.mul, 3, O.add, 4, O.sub, 10]
    >>> evaluate(hand)
    True
    '''
    result = hand[0]
    for i in range(1, len(hand), 2):
        op, value = hand[i:i+2]
        result = op(result, value)
        if result < 0: return False
    return (result == total)

def format_hand(hand):
    ''' Given a hand of numbers and operations, format
    it as a standard mathimatical expression.

    :param hand: The hand to format and print out
    :returns: The formatted mathimatical expression

    >>> hand = [11, O.sub, 2, O.mul, 3, O.add, 4, O.sub, 10]
    >>> format_hand(hand)
    11 - 2 × 3 + 4 - 10 = 21
    '''
    message = []
    for card in hand:
        if callable(card):
            message.append(OP_STRINGS[card])
        else: message.append(str(card))
    message.append('= 21')
    return ' '.join(message)

#------------------------------------------------------------
# solution
#------------------------------------------------------------

def find_card_hands(cards):
    ''' Given a hand of cards, return all the valid
    solutions to the stated problem.

    :param cards: The cards to find a solution for
    :returns: A generator for the solutions to the problem
    '''
    total = len(cards) + 4
    queue = [[cards[0]]]

    while queue:
        hand = queue.pop()
        if len(hand) == total:
            if evaluate(hand): yield hand
        else:
            card = cards[len(hand) // 2 + 1]
            for op in OPERATIONS:
                queue.insert(0, hand + [op, card])

#------------------------------------------------------------
# main
#------------------------------------------------------------
HAND = [11,2,3,4,10]
HAND = [2,10,3,8,11]

if __name__ == "__main__":
    for hand in find_card_hands(HAND):
        print format_hand(hand)

