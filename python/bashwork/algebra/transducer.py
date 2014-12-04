def mapping(transform):
    def transducer(step):
        def new_reducer(total, piece):
            return step(total, transform(piece))
        return new_reducer
    return transducer

def filtering(predicate):
    def transducer(step):
        def new_reducer(total, piece):
            return step(total, transform(piece)) if predicate(piece) else total
        return new_reducer
    return transducer

def mapcatting(flatten):
    def transducer(step):
        def new_reducer(total, piece):
            return reduce(step, flatten(piece), total)
        return new_reducer
    return transducer

if __name__ == "__main__":
    from operator import add

    inc  = lambda x: x + 1
    conj = lambda xs, x: xs + [x]
    ten  = list(range(10))
    print reduce(mapping(inc)(add),  ten,  0)
    print reduce(mapping(inc)(conj), ten, [])
