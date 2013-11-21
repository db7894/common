import sys

def rain_trap(heights):
    ''' Given a collection of building heights, figure
    out how much water would be trapped if rain fell from
    above and filled in the local minima.

    >>> heights = [0,1,0,2,1,0,1,3,2,1,2,1]
    >>> rain_trap(heights)
    6

    :param heights: An array of building heights
    :returns: The amount of water trapped
    '''
    def running_max(xs):
        rmax = -sys.maxint
        for x in xs:
            rmax = max(rmax, x)
            yield rmax
    rs = running_max(heights)
    ls = running_max(heights[::-1])
    ds = [min(r,l) for r,l in zip(rs, reversed(list(ls)))]
    return sum(d - h for d,h in zip(ds, heights))

def sub_strings(string, words):
    ''' Given a string composed of substrings, generate
    the list of substrings that exist in the word list.

    >>> words = [line.strip() for line in open('/usr/share/dict/words', 'r')]
    >>> words = set(words)
    >>> word = "thisfriendbestshould"
    >>> list(sub_strings(word, words))[-1]
    'this friend best should '

    :param string: The string to split in substrings
    :param words: The word list to look up strings in
    :returns: The substring pieces
    '''
    if not string: yield ''
    for i in range(len(string) + 1):
        head = string[:i]
        if head in words:
            for rest in sub_strings(string[i:], words):
                yield head + " " + rest

def longest_word_from_dict(pieces, words):
    ''' Given a list of word pieces (say the elements from
    the periodic table), what is the longest word from the
    supplied dictionary that we can create.

    >>> words  = [line.strip() for line in open('/usr/share/dict/words', 'r')]
    >>> words  = sorted(words, key=lambda w: len(w), reverse=True)
    >>> pieces = [line.split(',')[2].lower() for line in open('/opt/datasets/elements.csv', 'r')][1:]
    >>> longest_word_from_dict(pieces, words)
    'nonrepresentationalism -> n o n re p re se n ta ti o n al i sm '

    :param pieces: The word pieces that we can construct with
    :param words: The words that we are trying to create
    :returns: The first (longest) word we can create or None
    '''
    for word in words:
        for head in sub_strings(word, pieces):
            return word + " -> " + head
    return None

def stock_market(prices):
    ''' Given a line of ticker prices, if you could go back
    to any point and buy one share of stock and sell it after
    that, what is the maximum amount of money you could make.
    >>> prices = [1,2,3,4,5,6,7,8,9]
    >>> stock_market(prices)
    (8, 0, 8)

    >>> prices = [4,5,6,2,1,6,7,8,7]
    >>> stock_market(prices)
    (7, 4, 7)

    :param prices: An array of prices (index is time)
    :return: (max-return, day-to-buy, day-to-sell)
    '''
    iterprices = prices.items if isinstance(prices, dict) else enumerate(prices)
    max_return = (-sys.maxint, 0, 0)
    min_global = (sys.maxint, 0)
    for day, price in iterprices:
        min_global = min(min_global, (price, day))
        cur_return = price - min_global[0]
        max_return = max(max_return, (cur_return, min_global[1], day))
    return max_return


if __name__ == "__main__":
    import doctest
    doctest.testmod()
