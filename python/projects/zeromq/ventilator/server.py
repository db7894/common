import zmq
import random
import time

context = zmq.Context()

sender = context.socket(zmq.PUSH)
sender.bind("tcp://*:5557")

sink = context.socket(zmq.PUSH)
sink.connect("tcp://localhost:5558")

print "Press enter to start"
_ = raw_input()

sink.send('0')
random.seed()

total = 0
for task_nbr in range(100):
    workload = random.randint(1,100)
    total += workload
    sender.send(str(workload))
print "total cost %s ms" % total
time.sleep(1)
