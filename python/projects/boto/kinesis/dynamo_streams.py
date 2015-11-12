import boto3

def get_record_stream(**kwargs):
    ''' This works by first catching up with every record that 
    exists. After we have caught up, we simply poll for latest
    and then return what we have found.

    :param shard_type: The type of iterator to get (LATEST does no catch up)
    :returns: A generator around record streams
    '''
    shard_type = kwargs.get('shard_type', 'TRIM_HORIZON')
    table_name = kwargs.get('table_name', None)

    client  = boto3.client('dynamodbstreams')
    streams = client.list_streams()
    streams = streams['Streams']

    if table_name:
        streams = [stream for stream in streams['Streams'] if stream['TableName'] == table_name]

    stream  = client.describe_stream(StreamArn=streams['0']['StreamArn'])
    stream  = stream['StreamDescription']
    shards  = sorted(stream['Shards'], key=lambda s: s['SequenceNumberRange']['StartingSequenceNumber'])

    for shard in shards:
        params   = {
            'ShardId'           : shard['ShardId'],
            'StreamArn'         : stream['StreamArn'], 
            'ShardIteratorType' : shard_type, 
        }
        iterator = client.get_shard_iterator(**params)
        iterator = iterator['ShardIterator']
        records  = shard_type != 'LATEST'
        
        while records:
            records = client.get_records(ShardIterator=iterator, Limit=10)
            for record in records['Records']:
                yield record
            iterator = records['NextShardIterator']

    while iterator:
        records = client.get_records(ShardIterator=iterator, Limit=1)
        for record in records['Records']:
            yield record
        iterator = records['NextShardIterator']

class Operations(object):
    '''
    '''
    Insert = 'INSERT'
    Remove = 'REMOVE'
    Modify = 'MODIFY'

def parse_stream_record(record):
    ''' Parses the stream record and returns the modifications made
    to the database. The following cases exist:

    * (old, None) - The record has been deleted
    * (None, new) - The record has been inserted
    * (old,  new) - The record has been modified

    For the underlying schema, feel free to read the documentation:
    http://docs.aws.amazon.com/dynamodbstreams/latest/APIReference/API_Types.html

    :returns: (operation, old_record, new_record)
    '''
    def cleanup(r):
        return { k : r[k].values()[0] for k in r } if r else None

    operation = record['eventName']
    old_item  = record['dynamodb'].get('OldImage', None)
    new_item  = record['dynamodb'].get('NewImage', None)
    return (operation, cleanup(old_item), cleanup(new_item))

if __name__ == "__main__":
    from pprint import pprint as pretty_print

    for record in get_record_stream():
        operation, old_item, new_item = parse_stream_record(record)
        print "next record {}:\n".format(operation)
        if old_item: pretty_print(old_item); print
        if new_item: pretty_print(new_item); print
