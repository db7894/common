import binascii
from hashlib import sha256

def _iterate_pairs(seq):
    shared = iter(seq)
    return zip(shared, shared)

def calculate_merkle_hash(handle, hasher=sha256, chunk_size=1024 * 1024):
    '''
    .. code-block::

        calculate_merkle_hash(open(__NAME__, 'rb'))
    '''
    chunks = []
    for chunk in iter(lambda: handle.read(chunk_size), b''):
        chunks.append(hasher(chunk).digest())
    if not chunks:
        return hasher('').hexdigest()
    while len(chunks) > 1:
        new_chunks = []
        for first, second in _iterate_pairs(chunks):
            if not second: new_chunks.append(first)
            else: new_chunks.append(hasher(first + second).digest())
        chunks = new_chunks
    return binascii.hexlify(chunks[0])

def check_merkel_hash(handle, value):
    return value == calculate_merkel_hash(handle)
