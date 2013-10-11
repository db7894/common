import sys

#------------------------------------------------------------
#============================================================
class ChainOfResponsibility(object):
    ''' A simple implementation of the chain of
    responsibility (in a more functional style)
    '''

    def __init__(self, chains=None):
        self.chains = chains or []

    def add_chain(self, chain):
        ''' Add a chain handler which is a function that:
        
        * returns True if handled
        * returns False to pass control up the chain
        '''
        self.chains.append(chain)

    def handle_any(self, command):
        for chain in self.chains:
            if chain(command): return True
        return False

    def handle_all(self, command):
        results = [chain(command) for chain in self.chains]
        return all(results)

    @staticmethod
    def _test():
        chain = ChainOfResponsibility()
        chain.add_chain(lambda m: sys.stdout.write(m) or True)
        chain.add_chain(lambda m: sys.stderr.write(m) or True)
        chain.handle_any("error\n")
        chain.handle_all("error\n")
