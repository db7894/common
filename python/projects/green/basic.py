# -*- coding: utf-8 -
'''
Run with::

  gunicorn --workers=2 --debug --bind=localhost:8080 basic:app
'''

def app(environ, start_response):
    """Simplest possible application object"""
    data = 'Hello, World!\n'
    status = '200 OK'
    response_headers = [
        ('Content-type','text/plain'),
        ('Content-Length', str(len(data)))
    ]
    start_response(status, response_headers)
    return iter([data])
