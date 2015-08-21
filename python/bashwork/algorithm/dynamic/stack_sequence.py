def stack_match(values, match):
    ''' Given a collection of inputs with a specific
    ordering, is there a combination of interleaved
    push and pop stack operations that will achieve the
    supplied output match.

    TODO this doesn't work if the input values are not
    unique.

    >>> values = [1, 2, 3, 4, 5]
    >>> match = [4, 5, 3, 2, 1]
    >>> stack_match(values, match)
    True
    '''
    stack = []
    for value in values:
        stack.append(value)
        while stack and match and (stack[-1] == match[0]):
            stack.pop()
            match.pop(0)
    return (len(stack) == 0) and (len(match) == 0)
