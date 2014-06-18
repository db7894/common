import os
import redis
import urlparse
import logging
from werkzeug.wrappers import Request, Response
from werkzeug.routing import Rule, Map
from werkzeug.exceptions import HTTPException, NotFound
from werkzeug.wsgi import SharedDataMiddleware
from werkzeug.utils import redirect
from jinja2 import Environment, FileSystemLoader


#------------------------------------------------------------
# Helper Methods
#------------------------------------------------------------
def is_valid_url(url):
    parts = urlparse.urlparse(url)
    return parts.scheme in ('http', 'https')

def get_hostname(url):
    return urlparse.urlparse(url).netloc

def base36_encode(number):
    assert number >= 0, 'positive integer required'
    if number == 0:
        return '0'
    base36 = []
    while number != 0:
        number, i = divmod(number, 36)
        base36.append('0123456789abcdefghijklmnopqrstuvwxyz'[i])
    return ''.join(reversed(base36))


#------------------------------------------------------------
# Service Application
#------------------------------------------------------------
class Shortly(object):

    def __init__(self, database, template):
        ''' Initialize a new instance of the application
        
        :param database: The database to operate with
        :param template: The template environment manager
        '''
        self.database = database
        self.template = template
        self.url_map  = Map([
            Rule('/', endpoint='new_url'),
            Rule('/<short_url>', endpoint='follow_short_link'),
            Rule('/<short_url>+', endpoint='short_link_details')
        ])

    #------------------------------------------------------------
    # views
    #------------------------------------------------------------
    def on_error_404(self):
        '''
        '''
        response = self.render_template('404.html')
        response.status_code = 404
        return response

    def on_short_link_details(self, request, short_url):
        '''
        '''
        link_target = self.database.get('url-target:' + short_url)
        if link_target is None: raise NotFound()
        click_count = int(self.database.get('click-count:' + short_url) or 0)
        return self.render_template('short_link_details.html',
            link_target=link_target,
            short_url=short_url,
            click_count=click_count)

    def on_new_url(self, request):
        '''
        '''
        error = None
        url   = ''
        if request.method == 'POST':
            url = request.form['url']
            if is_valid_url(url):
                short_url = self.insert_url(url)
                return redirect('/%s+' % short_url)
            else: error = "please supply a valid url"
        return self.render_template('new_url.html', error=error, url=url)

    def on_follow_short_link(self, request, short_url):
        '''
        '''
        link_target = self.database.get('url-target:' + short_url)
        if link_target is None: raise NotFound()
        self.database.incr('click-count:' + short_url)
        return redirect(link_target)

    #------------------------------------------------------------
    # database
    #------------------------------------------------------------
    def insert_url(self, url):
        '''
        '''
        short_url = self.database.get('reverse-url:' + url)
        if short_url is not None: return short_url
        url_num = self.database.incr('last-url-id')
        short_url = base36_encode(url_num)
        self.database.set('url-target:' + short_url, url)
        self.database.set('reverse-url:' + url, short_url)
        return short_url

    #------------------------------------------------------------
    # helper methods
    #------------------------------------------------------------
    def dispatch_request(self, request):
        ''' A helper method to dispatch requests from the url
        router.

        :param request: The request to handle
        :returns: The result of handling the request
        '''
        adapter = self.url_map.bind_to_environ(request.environ)
        try:
            endpoint, values = adapter.match()
            return getattr(self, 'on_' + endpoint)(request, **values)
        except NotFound, ex:
            return self.on_error_404()
        except HTTPException, ex:
            return ex

    def render_template(self, template_name, **context):
        '''
        '''
        template = self.template.get_template(template_name)
        return Response(template.render(context), mimetype='text/html')
    
    def wsgi_app(self, environ, start_response):
        '''
        '''
        request  = Request(environ)
        response = self.dispatch_request(request)
        return response(environ, start_response)
    
    def __call__(self, environ, start_response):
        '''
        '''
        return self.wsgi_app(environ, start_response)

def create_application(**kwargs):
    ''' Create a new instance of the application

    :param db_host: The host of the database to use
    :param db_port: The port of the database to use
    :param with_static: A flag indicating if static should be used
    '''
    ap_root = os.path.dirname(__file__)
    db_host = kwargs.get('db_host', 'localhost')
    db_port = kwargs.get('db_port', 6379)
    config = {
        '/static'    : os.path.join(ap_root, 'static'),
        '/templates' : os.path.join(ap_root, 'templates'),
    }

    if kwargs.get('debug', True):
        logging.getLogger().setLevel(logging.DEBUG)
        logging.basicConfig()

    database = redis.Redis(db_host, db_port)
    loader   = FileSystemLoader(config['/templates'])
    template = Environment(loader=loader, autoescape=True)
    template.filters['hostname'] = get_hostname
    instance = Shortly(database, template)

    if kwargs.get('with_static', True):
        instance.wsgi_app = SharedDataMiddleware(instance.wsgi_app, config)
    return instance
