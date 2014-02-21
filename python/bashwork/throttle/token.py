'''
TODO
* make functional for CAS implementations (redis)
'''
import struct
import logging
from datetime import datetime, timedelta

#------------------------------------------------------------
# Logger Configuration
#------------------------------------------------------------
logger = logging.getLogger(__name__)

#------------------------------------------------------------
# Interfaces
#------------------------------------------------------------

class TokenBucket(object):
    ''' A simple interface for a token bucket implementation.
    Using the implementation is as simple as the following::

        bucket = ... an initialized token bucket
        if bucket.consume(user, count=2):
            execute(operation, user)
        else: handle(throttled, user)
    '''

    def consume(self, identifier, count=1):
        ''' Attempt to consume the supplied number of tokens for the
        supplied identifier.

        :param identifier: The identifier to consume tokens for
        :param count: The number of tokens to consume (default 1)
        :returns: True if tokens were available, False otherwise
        '''
        raise NotImplemented("consume")

class RefillPolicy(object):
    ''' An interface to a policy used to refill a given token
    bucket entry.
    '''

    def refill(self, entry):
        ''' Refill the supplied entry based on the underlying
        policy.

        :param entry: The entry to perform the refill for
        '''
        raise NotImplemented("consume")

#------------------------------------------------------------
# Refill Policies
#------------------------------------------------------------

class GenericRatePolicy(RefillPolicy):
    ''' A generic rate policy where a refill policy can be injected
    as a strategy to complete the interface.
    '''
    
    def __init__(self, strategy):
        ''' Initializes a new instance of the GenericRatePolicy

        :param strategy: The generic refill strategy to use
        '''
        self.strategy = strategy
    
    def refill(self, entry):
        ''' Refill the supplied entry based on the underlying
        policy.

        :param entry: The entry to perform the refill for
        '''
        self.strategy(entry)

class LinearRatePolicy(RefillPolicy):
    
    def __init__(self, period, rate):
        ''' Initialize a new instance of the LinearRatePolicy

        :param period: The period between updates
        :param rate: The rate per period of updates
        '''
        self.period = period.total_seconds() * 1000
        self.rate   = rate
    
    def refill(self, entry):
        ''' Refill the supplied entry based on the underlying
        policy.

        :param entry: The entry to perform the refill for
        '''
        now        = datetime.utcnow()
        interval   = (now - entry.timestamp).total_seconds() * 1000
        new_tokens = (self.rate * interval) / self.period
        
        entry.count += int(new_tokens)
        entry.timestamp = now

class StrictRatePolicy(RefillPolicy):
    
    def __init__(self, period, rate):
        ''' Initialize a new instance of the StrictRatePolicy

        :param period: The period between updates
        :param rate: The rate per period of updates
        '''
        self.period = period
        self.rate   = rate
    
    def refill(self, entry):
        ''' Refill the supplied entry based on the underlying
        policy.

        :param entry: The entry to perform the refill for
        '''
        now = datetime.utcnow()
        cutoff = entry.timestamp + self.period
        new_tokens = rate if now >= cutoff else 0
        
        #
        # We set the time to the cutoff instead of now as we want
        # to refill the counter ever period T.  If now happens to
        # be after T (t), the next timeout would be (T + t).
        #
        entry.count += new_tokens
        entry.timestamp = cutoff

class TokenRefillPolicy(object):
    ''' A collection of token policies for refilling
    a token bucket entry.
    '''

    @staticmethod
    def linearRate(period, rate):
        ''' A token refill policy that will refill tokens at a specified
        rate of `N` tokens every `T` time period: `tokens += (R * T)`. It
        should be noted that if a client calls in some point in the
        middle of a the time period, say `T / 2`, they will receive
        `(R * T) / 2` tokens::
        
            y = R * (t_2 - t_1) / T
        
        :param period: The period over which to refill to bucket.
        :param rate: The number of tokens to add during this period.
        :returns: A populated refill policy.
        '''
        assert rate > 0, "The refill rate must be greater than zero"
        return LinearRatePolicy(period, rate);

    @staticmethod
    def strictRate(period, rate):
        ''' A token refill policy that will refill the requested number of
        tokens at the specified time period. It should be noted that 
        tokens will only be refilled after the complete time period has
        elapsed:: 
        
            y = impulse(T) * R
        
        :param period: The period over which to refill to bucket.
        :param rate: The number of tokens to add during this period.
        :returns: A populated refill policy.
        '''
        assert rate > 0, "The refill rate must be greater than zero"
        return StrictRatePolicy(period, rate)

    @staticmethod
    def never():
        ''' A refill policy that will never refill a token bucket once
        all of its tokens have been used.
        
        :return: A populated refill policy.
        '''
        def policy(entry):
            entry.timestamp = datetime.utcnow()

        return GenericRefillPolicy(policy)
    
    @staticmethod
    def always(final long count) {
        ''' A refill policy that will always keep a bucket refilled
        to the specified count regardless of how often it is called.
        
        :param count: The count to keep the supplied cache at.
        :return: A populated refill policy.
        '''
        def policy(entry):
            entry.count = count
            entry.timestamp = datetime.utcnow()

        return GenericRefillPolicy(policy)

#------------------------------------------------------------
# Containers
#------------------------------------------------------------

class TokenBucketEntry(object):
    ''' A container to store the current status of a given
    token bucket entry.
    '''
    __slots__ = ['identifier', 'timestamp', 'count']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the TokenBucketEntry

        :param identifier: The identifier associated with this entry
        :param timestamp: The timestamp of the last update
        :param count: The number of tokens associated with this entry
        '''
        self.identifier = kwargs.get('identifer')
        self.timestamp  = kwargs.get('timestamp', None) or datetime.utcnow()
        self.count      = kwargs.get('count', 0)

class TokenBucketConfig(object):
    ''' A container to store the configuration for
    a given token bucket.
    '''
    __slots__ = ['max_tokens', 'min_tokens', 'time_to_live', 'timeout', 'start_tokens', 'refill_policy']

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the TokenBucketConfig

        :param max_tokens: The maximum number of tokens per entry
        :param min_tokens: The minimum number of tokens per entry
        :param time_to_live: The maximum time for each entry to live (timedelta)
        :param timeout: The maximum time to wait for a remove service (timedelta)
        :param start_tokens: The number of tokens for a new entry to start with
        :param refill_policy: The refill policy to use for repopulation
        '''
        self.max_tokens    = kwargs.get("max_tokens", 1)
        self.min_tokens    = kwargs.get("min_tokens", 0)
        self.start_tokens  = kwargs.get("start_tokens", 1)
        self.time_to_live  = kwargs.get("time_to_live", timedelta(hours=1)).total_seconds()
        self.timeout       = kwargs.get("time_to_live", timedelta(seconds=15)).total_seconds()
        self.refill_policy = kwargs.get("refill_policy", TokenBucketPolicy.never())

#------------------------------------------------------------
# Abstract Implementation
#------------------------------------------------------------

class AbstractTokenBucket(TokenBucke):
    '''
    A token bucket base class that can be adapted to new
    underlying persistence mechanisms simply by extending and
    implementing the setEntry and getEntry methods.
    '''
    
    def __init__(self, **kwargs):
        ''' Initialize a new instance of the AbstractTokenBucket
        
        :param config: The underlying token bucket configuration.
        '''
        self.config = kwargs.get('config')
    
    def consume(self, identifier, count):
        ''' Consume a specified number of tokens for the given
        identifier.
        
        :param identifier: The entity to consume tokens for.
        :param count: The number of tokens to consume.
        :returns: True if the operation can proceed, False otherwise.
        '''
        entry = get_entry(identifier)
       
        # 
        # The policy takes care of updating the token count and
        # updating the time stamp of the update.
        # 
        config.refill_policy.refill(entry)        
        new_count = min(entry.count, config.max_tokens) - count;
    
        #
        # There is no need to update the remote values as we have not subtracted
        # any tokens. The next time the request is made, the calculation will
        # simply be repeated.
        #
        if new_count < config.min_tokens:
            return False
    
        entry.count = new_count
        return set_entry(entry)

    def get_entry(self, identifier):
        ''' Retrieve the TokenBucketEntry for the supplied identifier.
        
        :param identifier: The entity to retrieve the TokenBucketEntry for.
        :returns: The TokenBucketEntry for the given identifier.
        '''
        raise NotImplemented("get_entry")
        
    def set_entry(self, entry):
        ''' Given a TokenBucketEntry, persist it.
        
        :param entry: The TokenBucketEntry to persist.
        :returns: True if successful, False otherwise.
        '''
        raise NotImplemented("set_entry")

#------------------------------------------------------------
# Implementations
#------------------------------------------------------------
class RedisTokenBucket(AbstractTokenBucket):
    ''' Note, this token bucket is currently not
    safe as there is a race condition between the get
    operation and the update.
    '''
    
    EPOCH = datetime.utcfromtimestamp(0)

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the RedisTokenBucket class.
        
        :param prefix: The prefix used for these redis entries
        :param redis: The redis instance used for persistence
        :param config: The underlying token bucket configuration
        '''
        super(RedisTokenBucket, self).__init__(**kwargs)
        self.redis  = kwargs.get('redis')
        self.prefix = kwargs.get('prefix', '');

    def get_entry(self, identifier):
        ''' Retrieve the TokenBucketEntry for the supplied identifier.
        
        :param identifier: The entity to retrieve the TokenBucketEntry for.
        :returns: The TokenBucketEntry for the given identifier.
        '''
        try
            key = self.prefix + identifier
            serialized = self.redis.get(key)    
            if serialized == None:
                return TokenBucketEntry(identifier, datetime.utcnow(), self.config.start_tokens)
            
            count, timestamp = struck.unpack("LL", serialized)
            timestamp = datetime.utcfromtimestamp(timestamp / 1000.0)
            return TokenBucketEntry(identifier, timestamp, count)
        
        except RedisError, ex:
            #
            # If somehow we get a fault, we do not want the entire service to go
            # down. Therefore, we will simply allow everything through and then
            # allow clients to be reset.
            #
            logger.error("Error getting entry {}".format(identifier))
            return TokenBucketEntry(identifier, datetime.utcnow(), self.config.start_tokens)
        
    def set_entry(self, entry):
        ''' Given a TokenBucketEntry, persist it.
        
        :param entry: The TokenBucketEntry to persist.
        :returns: True if successful, False otherwise.
        '''
        key = self.prefix + entry.identifier
        timestamp = (entry.timestamp - self.EPOCH).total_seconds() * 1000
        serialized = struct.pack('LL', entry.count, timestamp)
        
        try:
            return self.redis.set(key, serialized, config.time_to_live)
        except: RedisError, ex:
            logger.error("Error setting entry {}".format(entry.identifier))
            return False
