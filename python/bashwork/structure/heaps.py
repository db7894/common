import heapq
from bashwork.generators import gen_file_stream

class HeapedStreams(object):

    @classmethod
    def create(klass, **kwargs):
        ''' Initialize the HeapedStreams class

        :param streams: The streams to merge
        :param files: The files to merge
        :returns: An initialized heaped stream collection
        '''
        streams = kwargs.get('streams', []) + kwargs.get('files', [])
        streams = { i: gen_file_stream(s) for i, s in enumerate(streams) }
        heap = klass(streams=streams)
        heap.repopulate()
        return heap

    def __init__(self, streams):
        ''' Initialize a new instance of the HeapedStreams class.

        :param streams: The collection of streams to merge
        '''
        self.streams = streams
        self.heap    = []

    def repopulate(self, stream=None):
        ''' This will make sure all the heaps have at least
        one item available to pull from
        '''
        if stream == None: streams = self.streams.items()
        elif stream not in self.streams: return
        else: streams = [(stream, self.streams[stream])]

        for idx, stream in streams:
            try:
                heapq.heappush(self.heap, (stream.next(), idx))
            except StopIteration: del self.streams[idx]
   
    def next(self):
        if not len(self.heap):
            raise StopIteration()

        entry, stream = heapq.heappop(self.heap)
        self.repopulate(stream)
        return entry

    def __iter__(self): return self

if __name__ == "__main__":
    files = ['a', 'b', 'c', 'd']
    heap  = HeapedStreams.create(files=files)
    for entry in heap:
        print entry
