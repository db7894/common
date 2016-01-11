class Something(object):
    __slots__ = ['a', 'b', 'c']

    def __init__(self, a, b, c):
        self.a = a
        self.b = b
        self.c = c

    def __copy__(self):
        return type(self)(**{ k : getattr(self, k) for k in self.__slots__ })
