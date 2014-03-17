import hashlib

TRANS_5C = "".join(chr(x ^ 0x5c) for x in xrange(256))
TRANS_36 = "".join(chr(x ^ 0x36) for x in xrange(256))

def hmac_sign(key, message, method=hashlib.md5):
    ''' Given a key and a message, hmac sign them
    with the supplied hash method (default md5)

    :param key: The secret key to sign with
    :param message: The message to hmac sign
    :param method: The hash method to be used
    :returns: The hmac digest of the message
    '''
    blocksize = method().block_size
    if len(key) > blocksize:
        key = method(key).digest()
    key += chr(0) * (blocksize - len(key))
    o_key_pad = key.translate(TRANS_5C)
    i_key_pad = key.translate(TRANS_36)
    return method(o_key_pad + method(i_key_pad + message).digest()).digest()

if __name__ == "__main__":
    import sys
    digest = hmac_sign(sys.argv[1], sys.argv[2])
    print digest.hexdigest()
