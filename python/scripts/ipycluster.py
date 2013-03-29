"""
Start with `ipcluster`
"""
#------------------------------------------------------------
# Initialize the connection to the client
#------------------------------------------------------------
# Also look at StarCluster to use EC2 instances
#------------------------------------------------------------
from IPython.parallel import Client
client = Client()
queue = client.direct_view()
print "available workers: ", len(queue)

#------------------------------------------------------------
# Do some work in parallel
#------------------------------------------------------------
squared = queue.map_sync(lambda x: x**2, [1,2,3,4])
print squared
