import sys
from common import Queue

#--------------------------------------------------------------------------------#
# make change
#--------------------------------------------------------------------------------#
def make_change_bfs(value, coins):
    ''' Given a collection of coins, return the smallest change
    needed to make the requested value.

    :param value: The value to make change for
    :param coins: The coins to use to make change with
    :returns: The coins needed to make change
    '''
    solved, queue = [], Queue()
    queue.enqueue((value, []))
    while not queue.is_empty():
        remain, change = queue.dequeue()
        if remain == 0: solved.append(change); continue
        for coin in coins:
            if remain - coin >= 0:
                queue.enqueue((remain - coin, [coin] + change))
    return min((len(s), s) for s in solved)[1]

def make_quick_change(value, count):
    ''' Given a collection of coins, return the smallest change
    needed to make the requested value.

    :param value: The value to make change for
    :param count: The current count of coins to change with
    :returns: The number of coins needed to make change
    '''
    if         value ==  0: return (len(count), count)
    if    0 <  value  < 10: return change(value - value, [1 for _ in range(value)] + count)
    elif 10 <= value  < 25: return change(value - 10, [10] + count)
    elif       value >= 25: return min(change(value - 10, [10] + count), change(value - 25, [25] + count))
    elif       value >= 50: return change(value - 50, [25, 25] + count) # + lookup table is O(1)
    else: raise ValueError("cannot make change for negative values")

def make_change_dp(value, coins):
    ''' Given a collection of coins, return the smallest change
    needed to make the requested value.

    :param value: The value to make change for
    :param coins: The coins to use to make change with
    :returns: The number of coins needed to make change
    '''
    Count, Graph = {0: 0}, {}
    for v in range(1, value + 1):
        minc, minv = -1, sys.maxint
        for coin in coins:
            if coin <= v and 1 + Count[v - coin] < minv:
                minc, minv = coin, 1 + Count[v - coin]
        Count[v], Graph[v] = minv, minc

    print "solution for %s:" % value,
    while value > 0:
        print Graph[value],
        value -= Graph[value]
    return Count

def general_solution(value):
    ''' Given a value above the maximum coin,
    use the O(1) solution with a lookup table to solve
    the problem.

    :param value: The value above 50 to solve
    :returns: The number of coins needed to make change
    '''
    table = make_change_dp(50, [1,5,10,25])
    count = 0
    while value >= 50:
        value, count = value - 50, count + 2
    return count + table[value]


#------------------------------------------------------------
# tests
#------------------------------------------------------------
if __name__ == "__main__":
    print general_solution(127)
