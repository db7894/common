class Lens(object):
    '''
    Lens = Functor f => (T -> f T) -> U -> f U
    U { T }

    view
    set
    over
    compose
    '''

    def __init__(self, getter, setter):
        self.getter = getter
        self.setter = setter

    def get(self, instance):
        return self.getter(instance)

    def set(self, instance, value):
        return self.setter(instance, value)

    @staticmethod
    def compose(a, b):
        getter = lambda o: b.get(a.get(o))
        setter = lambda o, v: a.set(o, b.set(a.get(o), v))
        return Lens(getter, setter)

if __name__ == "__main__":

    class Name(object):

        def __init__(self, **kwargs):
            self.first = kwargs.get('first')
            self.last  = kwargs.get('last')

        def __str__(self):
            return "{} {}".format(self.first, self.last)

    class Customer(object):

        def __init__(self, **kwargs):
            self.name = kwargs.get('name')
            self.age  = kwargs.get('age')

        def __str__(self):
            return "{} {}".format(self.name, self.age)

    last_lens = Lens(lambda o: o.last, lambda o, v: Name(last=v, first=o.first))
    name_lens = Lens(lambda o: o.name, lambda o, v: Customer(name=v, age=o.age))
    last_name_lens = Lens.compose(name_lens, last_lens)
    customer  = Customer(name=Name(first="galen", last="collins"), age=31)
    customer2 = last_name_lens.set(customer, "super-collins")

    assert( str(name_lens.get(customer))      == "galen collins" )
    assert( str(last_lens.get(customer.name)) == "collins" )
    assert( str(last_name_lens.get(customer)) == "collins" )
    assert( str(customer)                     == "galen collins 31" )
    assert( str(customer2)                    == "galen super-collins 31" )
