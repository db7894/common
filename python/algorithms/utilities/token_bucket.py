from time import time
from threading import RLock

class TokenBucket(object):
    ''' A simple method of rate limiting a task over some time.
    More information can be found here: http://en.wikipedia.org/wiki/Token_bucket

    TODO implement strategies
    '''

    def __init__(self, tokens, rate, timer=None):
        ''' Initialize a new instance of the TokenBucket

        :param tokens: The maximum amount of tokens to allow
        :param rate: The throttling rate to provide
        :param timer: The source of time provider
        '''
        self.max_tokens  = tokens
        self.cur_tokens  = tokens
        self.refill_rate = rate
        self.time_source = timer or time
        self.last_update = self.time_source()
        self.__lock = RLock()

    def get_tokens(self, tokens=1):
        ''' Attempt to request the specified number of
        tokens required for the task at hand.

        :params tokens: The number of tokens required (default 1)
        :returns: True if successful, False otherwise
        '''
        with self.__lock:
            self.__refill_tokens()
            if self.cur_tokens < tokens: return False
            self.cur_tokens -= tokens
            return True

    def time_for_tokens(self, tokens=1):
        ''' Return the time it will take to retrieve
        the required amount of tokens.

        :params tokens: The number of tokens required (default 1)
        :returns: The amount of time needed before the tokens are available
        '''
        needed = tokens - self.cur_tokens
        return needed * 1.0 / self.refill_rate

    def __refill_tokens(self):
        ''' Called to refill the underlying token collection
        before the next token request.
        '''
        now = self.time_source()
        update = int((now - self.last_update) * self.refill_rate + 0.5)
        self.cur_tokens = min(self.cur_tokens + update, self.max_tokens)
        self.last_update = now

if __name__ == "__main__":
    from time import sleep

    tokens = 50
    bucket = TokenBucket(100, 10)
    bucket.get_tokens(tokens)
    for i in range(10):
        while not bucket.get_tokens(tokens):
            print "waiting to get {} tokens".format(tokens)
            print bucket.cur_tokens
            sleep(bucket.time_for_tokens(tokens))
        print "Got next round of {} tokens".format(tokens)
