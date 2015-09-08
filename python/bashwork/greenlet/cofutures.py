import time
import inspect
from concurrent.future import Future
from concurrent.futures import ThreadPoolExecutor
from concurrent.futures import ProcessPoolExecutor

class Task(Future):
    '''

    .. code-block:: python

        pool = ThreadPoolExecutor(max_workers=4)

        def function(x, y):
            time.sleep(1)
            return x + y

        @inlined_future
        def do_function(x, y):
            result = yield pool.submit(function, x, y)
            return result

        task = Task(do_function(2, 3))
        task.step()
        print("result: ", task.result())
    '''

    def __init__(self, generator):
        super().__init__()
        self.generator = generator

    def step(self, value=None, exception=None):
        try:
            if exception:
                future = self.generator.throw(exception)
            else:
                future = self.generator.send(value)
            future.add_done_callback(self.wakeup)
        except StopIteration as ex:
            self.set_result(ex.value)

    def wakeup(self, future):
        try:
            result = future.result()
            self.step(value=result)
        except Exception as ex:
            self.step(exception=ex)

#--------------------------------------------------------------------------------
# Coroutine Helper Methods
#--------------------------------------------------------------------------------

def patch_future(klass):
    ''' This modifies the current future class to make
    yield from work the same as yield.
    '''
    def __iter__(self):
        if not self.done():
            yield self
        return self.result()
    klass.__iter__ = __iter__
patch_future(Future)

def create_inline_future(future):
    task = Task(future)
    task.step()
    return task

def execute_inline_future(future):
    '''
    .. code-block:: python

        result = execute_inline_future(do_function(2, 3))
        print("got: ", result)
    '''
    task = create_inline_future(future)
    return task.result()

def inlined_future(function):
    ''' A simple lint check as well as a marker for
    a inline future function.
    '''
    assert inspect.isgeneratorfunction(function)
    return function

#--------------------------------------------------------------------------------
# Coroutine Utility Methods
#--------------------------------------------------------------------------------

@inlined_future
def after(delay, generator):
    '''
    Task(after(10, do_function(2, 3))).step()
    '''
    yield from pool.submit(time.sleep, delay)
    yield from generator


if __name__ == "__main__": pass
