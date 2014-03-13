#--------------------------------------------------------------------------------#
#  dna alignment
#--------------------------------------------------------------------------------#
def dna_align(this, that):
    ''' Given two strings, find their best alignment::

        acts = { 's':=, 'n':i_, 'm':j_, 'e':!= }

    :param this: The first string to compare
    :param that: The second string to compare
    '''
    n, m = len(this) - 1, len(that) - 1
    acts, edits = {}, {}
    for i in range(n + 1): edits[i,0] = i
    for j in range(m + 1): edits[0,j] = j
    for i in range(1, n + 1):
        for j in range(1, m + 1):
            if this[i] != that[j]:
                edit, act = min((edits[i - 1, j], 'm'), (edits[i, j - 1], 'n'), (edits[i - 1, j - 1], 'e'))
                acts[i, j], edits[i,j] = act, 1 + edit
            else: acts[i, j], edits[i, j] = 's', edits[i - 1, j - 1]

    print "cost %d" % edits[n,m]
    nalign, malign = [], []
    while n > 0 or m > 0:
        if   acts[n, m] == 's' or acts[n, m] == 'e':
            nalign.append(this[n])
            malign.append(that[m])
            n, m = n - 1, m - 1
        elif acts[n, m] == 'n':
            nalign.append(' ')
            malign.append(that[m])
            n, m = n, m - 1
        elif acts[n, m] == 'm':
            nalign.append(this[n])
            malign.append(' ')
            n, m = n - 1, m

        if n == 0:
            nalign = nalign + [' '] * m
            malign = malign + list(that[0:m])
            n = m = 0
        elif m == 0:
            nalign = nalign + list(this[0:n])
            malign = malign + [' '] * n
            n = m = 0

    print ''.join(reversed(nalign))
    print ''.join(reversed(malign))

#------------------------------------------------------------
# tests
#------------------------------------------------------------
if __name__ == "__main__":
    this = 'GGAGTGAGGGGAGCAGTTGGCTGAAGATGGTCCCCGCCGA'
    that = 'CGCATGCGGAGTGAGGGGAGCAGTTGGGAACAGATGGTCC'
    dna_align(this, that)
