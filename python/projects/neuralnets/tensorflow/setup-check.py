import tensorflow as Flow

hello = Flow.constant('hello world')
session = Flow.Session()
print(session.run(hello))

a = Flow.constant(10)
b = Flow.constant(32)
print(session.run(a + b))
