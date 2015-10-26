I  = lambda x: x
K  = lambda x: lambda y: x
M  = lambda x: x(x)

def egocentric(x):
    assert x(x) == x

def fond(A, B):
    assert A(B) == B

def fixated(A, B, x):
    assert A(x) == B

def compose(A, B, C, x):
    assert C(x) == A(B(x))

#A = lambda x:
#B = lambda x:
#C = lambda x: A(B(x)
