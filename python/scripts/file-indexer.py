import os
from multiprocessing import Queue, Process

def crawler(queue, root, predicate):
    def crawl(path):
        for entry in os.listdir(path):
            entry = os.path.join(path, entry)
            if os.path.isdir(entry):
                crawl(entry)
            elif predicate(entry):
                queue.put(entry)
    crawl(root)

def indexer(queue):
    while True:
        if not queue.empty():
            print queue.get()
            #for line in open(entry, 'r'):
            #    print line,

queue = Queue()
path  = os.path.abspath('/home/')
predicate = lambda p: p.split('.')[-1] == "java"
Process(target=crawler, args=(queue, path, predicate)).start()
Process(target=indexer, args=(queue,)).start()
