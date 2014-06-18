from os import path

#------------------------------------------------------------
# Main Application Configuration
#------------------------------------------------------------

ROOT   = path.dirname(__file__)
config = {
    'debug'     : True,
    'root_path' : ROOT,
    'index_path': path.join(ROOT, path.join('var', 'index')),
    'cache_path': path.join(ROOT, path.join('var', 'cache')),
    'db_host'   : 'localhost',
    'db_port'   : 6379,
}
