import sys
import time
import types
import itertools
from random import random, seed
from functools import wraps
from datetime import datetime, timedelta

# ------------------------------------------------------------
# Exceptions
# ------------------------------------------------------------

class RetryException(Exception):
    ''' The base exception for the retryable
    decorator framework.
    '''
    pass

class TimeoutException(RetryException):
    ''' An exception that indicates that the
    retryable timeout period has elapsed.
    '''
    pass

# ------------------------------------------------------------
# Policy Callables
# ------------------------------------------------------------

class Backoff(object):
    ''' A collection of backoff policies that can be
    applied to a retry policy.
    '''

    @staticmethod
    def constant(delay):
        ''' A retry policy that will delay always for
        the same designated delay.

        :param delay: The constant amount to delay
        :returns: An initialized backoff policy callable
        '''
        def wrapper(retry):
            retry.backoff = delay
        return wrapper

    @staticmethod
    def linear(multiplier=1):
        ''' A retry policy that will increaase the delay
        timeout in a linear fashion.

        :param multipler: An extra multiplier to apply
        :returns: An initialized backoff policy callable
        '''
        def wrapper(retry):
            def backoff(count, policy):
                delay = policy.min_delay * count * multiplier
                return min(delay, policy.max_delay)

            if not retry.min_delay:
                retry.min_delay = 0.1
            retry.backoff = backoff
        return wrapper

    @staticmethod
    def exponential(coefficient=1.5):
        ''' A retry policy that will increase the delay
        timeout in an exponential fashion. This bases its calculation
        on the min_delay and max_delay parameters of the policy (which if
        not supplied become 0.1 and sys.maxint by default). The backoff 
        delay is calculated as::

            delay = min_delay * (coef ** (count - 1))
            delay = min(delay, max_delay)
            sleep(delay)

        :param coefficient: Controls the growth of the delay
        :returns: An initialized backoff policy callable
        '''
        def wrapper(retry):
            def backoff(count, policy):
                delay = policy.min_delay * (coefficient ** (count - 1))
                return min(delay, policy.max_delay)

            if not retry.min_delay:
                retry.min_delay = 0.1
            retry.backoff = backoff
        return wrapper


class Jitter(object):
    ''' A collection of jitter policies that can
    be applied to a retry policy to keep hosts
    unaligned and prevent stampedes.
    '''

    @staticmethod
    def uniform(spread=100):
        ''' A policy that will delay a retry using
        a uniforma spread (+/-) of jitter. This is
        calculated as::

            jitter = random.uniform(-spread, spread)
            delay  = delay + jitter
            sleep(delay)

        :param spread: The uniform spread of jitter to add
        :returns: An initialized jitter policy callable
        '''
        def wrapper(policy):
            def method(delay):
                scale = random.uniform(-spread, spread)
                return delay + scale

            seed() # just in case this is not done elsewhere
            policy.jitter = jitter
        return wrapper

    @staticmethod
    def random(coefficient=1):
        ''' A policy that will delay a retry using
        a random amount of jitter. This is calculated
        as::

            jitter = delay * (1 - (coef * random()))
            delay  = delay + jitter
            sleep(delay)

        :param coefficient: Controls the amount of jitter to apply
        :returns: An initialized jitter policy callable
        '''
        def wrapper(policy):
            def method(delay):
                scale = 1 - (coefficient * random())
                return delay * scale

            seed() # just in case this is not done elsewhere
            policy.jitter = jitter
        return wrapper

# ------------------------------------------------------------
# Retry Policy
# ------------------------------------------------------------

class Policy(object):
    ''' A context for maintaining the retry policy.
    '''

    def __init__(self, *args, **kwargs):
        ''' Initialize a new instance of the RetryPolicy

        :param jitter: An amount to jitter the backoff delay, or a callable (default None)
        :param backoff: An backoff value to delay for, or a callable (default 1)
        :param min_delay: The minimum delay to proceed with (default None)
        :param max_delay: The maximum delay to proceed with (default sys.maxint)
        :param predicate: A predicate to test the result with (default None)
        :param timeout: An absolute timeout to stop processing, or a callable (default None)
        :param retries: The number of times to retry the call (default None)
        :param exception: A single exception to catch (default all exceptions)
        :param exceptions: A list of exceptions to catch (default all exceptions)
        '''
        self.jitter     = kwargs.get('jitter',    None)
        self.backoff    = kwargs.get('backoff',   1)
        self.min_delay  = kwargs.get('min_delay', None)
        self.max_delay  = kwargs.get('max_delay', sys.maxint)
        self.predicate  = kwargs.get('predicate', None)
        self.timeout    = kwargs.get('timeout',   None)
        self.retries    = kwargs.get('retries',   None)
        if 'exception' in kwargs:
            self.exceptions = [kwargs.get('exception')]
        else: self.exceptions = kwargs.get('exceptions', None)

        # apply un-named policy arguments
        for policy in args:
            if callable(policy): policy(self)

    @classmethod
    def forever(klass, **kwargs):
        ''' A policy that will retry a call forever or
        until it succeeds::

            @retry(Policy.forever())
            def example(): pass

        :returns: An initialized retry policy
        '''
        kwargs['retries'] = None
        return klass(**kwargs)

    @classmethod
    def never(klass, **kwargs):
        ''' A policy that will not retry a call
        after it has failed::

            @retry(Policy.never())
            def example(): pass

        :returns: An initialized retry policy
        '''
        kwargs['retries'] = 0
        return klass(**kwargs)

    @classmethod
    def unless(klass, predicate, **kwargs):
        ''' A policy that will retry a call until the
        result satisfies the specified predicate::

            @retry(Policy.unless(lambda x: x % 2 == 0))
            def example():
                return random.randint(0, 100)

        :param predicate: The predicate to run until it is true
        :returns: An initialized retry policy
        '''
        kwargs['predicate'] = predicate
        return klass(**kwargs)

    @classmethod
    def unless_not(klass, predicate, **kwargs):
        ''' A policy that will retry a call until the
        result satisfies the inverse of the predicate.

            @retry(Policy.unless_not(lambda x: x % 2 == 0))
            def example():
                return random.randint(0, 100)

        :param predicate: The predicate to run until it isn't true
        :returns: An initialized retry policy
        '''
        kwargs['predicate'] = lambda x: not predicate(x)
        return klass(**kwargs)

    @classmethod
    def until_true(klass, **kwargs):
        ''' A policy that will retry a call until the
        result is true::

            @retry(Policy.until_true())
            def example():
                return random.choice([1, 2]) == 1

        :returns: An initialized retry policy
        '''
        kwargs['predicate'] = lambda x: x
        return klass(**kwargs)

    @classmethod
    def until_false(klass, **kwargs):
        ''' A policy that will retry a call until the
        result is false::

            @retry(Policy.until_false())
            def example():
                return random.choice([1, 2]) == 1

        :returns: An initialized retry policy
        '''
        kwargs['predicate'] = lambda x: not x
        return klass(**kwargs)

    @classmethod
    def until_time(klass, timeout, **kwargs):
        ''' A policy that will retry a call until the
        timeout period has been reached or until it
        succeeds::

            @retry(Policy.until_time(datetime.now() + timedelta(minutes=1))
            def example(operation):
                return faulty_client.execute(operation)

        :param timeout: The time to retry the operation until
        :returns: An initialized retry policy
        '''
        kwargs['timeout'] = timeout
        return klass(**kwargs)

    @classmethod
    def for_the_next(klass, delta, **kwargs):
        ''' A policy that will retry a call for the
        next specified delta amount of time or until
        it succeeds::

            @retry(Policy.for_the_next(timedelta(minutes=1))
            def example(operation):
                return faulty_client.execute(operation)

        :param delta: The time delta to retry the operation for
        :returns: An initialized retry policy
        '''
        kwargs['timeout'] = datetime.now() + delta
        return klass(**kwargs)

    @classmethod
    def times(klass, count, **kwargs):
        ''' A policy that will retry a call the specified
        number of times or until it succeeds::

            @retry(Policy.times(5))
            def example():
                return random.choice([1, 2]) == 1

        :param count: The number of times to retry the failed operation
        :returns: An initialized retry policy
        '''
        kwargs['retries'] = count
        return klass(**kwargs)

# ------------------------------------------------------------
# Method Decorator
# ------------------------------------------------------------

class retry(object):
    ''' A decorator that will allow a function to
    be retried based on the supplied critera.
    '''
    
    def __init__(self, policy=Policy.forever()):
        ''' Initializes a new instance of the retry decorator.

        @param policy The retry policy to operate with
        '''
        self.policy = policy
        self.current_delay = 0

    def handle_exception(self, ex):
        ''' Given an exception, check if it is handled based
        on the policy, and if not rethrow it. If the policy
        has not set any exception filters, then we handle
        all exceptions.

        :param ex: The exception that was raised for this operation
        :returns: True if the operation should continue
        :raises: ex
        '''
        if not self.policy.exceptions:
            return True

        for exception in self.policy.exceptions:
            if isinstance(ex, exception):
                return True
        raise ex # throw unhandled exceptions

    def handle_timeout(self):
        ''' Based on the policy, check if the operation has
        timed out, and if so throw an exception.

        :returns: True if the operation can continue
        :raises: TimeoutException
        '''
        if not self.policy.timeout:
            return True

        if datetime.now() > self.policy.timeout:
            message = "The timeout period specified has elapsed: {}"
            raise TimeoutException(message.format(self.policy.timeout))

    def handle_retries(self, count):
        ''' Check if the number of attempted method calls
        is greater than the specified retry count.

        :param count: The current retry count of this operation
        :returns: True if the operation can continue
        :raises: RetryException
        '''
        if not self.policy.retries:
            return True

        if count >= self.policy.retries:
            message = "The requested number of retries have been attempted: {}"
            raise RetryException(message.format(self.policy.retries))

    def handle_predicate(self, result):
        ''' Given a predicate, evaluate the method call
        result and see if we should continue processing
        or not.

        :param result: The result of the current operation
        :returns: True if the operation can continue, False otherwise
        '''
        if not self.policy.predicate:
            return True
        return self.policy.predicate(result)

    def handle_backoff(self, count):
        ''' Given a backoff algorithm, apply it and sleep
        the requested amount of time before continuing.

        :param count: The current retry count of this operation
        '''
        if not self.policy.min_delay:
            return True

        if self.current_delay == 0:
            self.current_delay = self.policy.min_delay
        elif callable(self.policy.backoff):
            self.current_delay = self.policy.backoff(count, self.policy)
        else: self.current_delay *= self.policy.backoff

        if callable(self.policy.jitter):
            self.current_delay = self.policy.jitter(self.current_delay)
        elif self.policy.jitter:
            self.current_delay *= self.policy.jitter

        time.sleep(self.current_delay)

    def __call__(self, callback):
        ''' The retryable wrapper for the method call.

        :param callback: The decorated method
        '''
        @wraps(callback)
        def wrapper(*args):
            for count in itertools.count(0):
                try:
                    result = callback(*args)
                    if self.handle_predicate(result):
                        return result
                except Exception, ex:
                    self.handle_exception(ex)
                self.handle_backoff(count)
                self.handle_timeout()
                self.handle_retries(count)
        return wrapper

# ------------------------------------------------------------
# Class Decorator
# ------------------------------------------------------------

class retry_meta(type):
    ''' A class based meta class that decorates all
    public methods with retryable policy::

        class ExampleClass(object):
            __metaclass__ = retry_meta
            __policy__    = Policy.forever()
    '''

    def __new__(klass, name, bases, attrs):
        ''' Apply the retryable method decorator to every
        public method in the class.
        '''
        policy = attrs['__policy__']
        for attr_name, attr_value in attrs.iteritems():
            if (isinstance(attr_value, types.FunctionType)
                and not attr_name.startswith('_')):
                attrs[attr_name] = retry(policy)(attr_value)
        return super(retry_meta, klass).__new__(klass, name, bases, attrs)
