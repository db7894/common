import os
import json
import urllib

class Freebase(object):
    '''
    '''

    def __init__(self):
        '''
        '''
        self.home = os.path.join(os.environ['HOME'], '.google-api-key').strip()
        self.api_key = open(self.home, 'r').read()
        self.service_url = 'https://www.googleapis.com/freebase/v1/mqlread'

    def request(self, query, cursor=''):
        '''
        '''
        params = {
            'query': json.dumps(query),
            'key': self.api_key,
            'cursor': cursor,
        }
        url = self.service_url + '?' + urllib.urlencode(params)
        response = json.loads(urllib.urlopen(url).read())
        return response['result'], response.get('cursor', None)

    def paged_request(self, query):
        '''
        '''
        result, cursor = self.request(query)
        while cursor:
            yield result
            result, cursor = self.request(query, cursor)

    def get_planets(self):
        '''
        '''
        query = [{'name': None, 'type': '/astronomy/planet'}]
        planets, cursor = self.request(query)
        return [planet['name'] for planet in planets]

    def get_movie_titles(self):
        '''
        '''
        query = [{
          "limit": 1000,
          "name": None,
          "type": "/film/film",
          "gross_revenue": [{
            "amount": None,
            "currency": "United States Dollar",
            "valid_date": None
          }],
          "sort": "-gross_revenue.amount"
        }]
        movies, cursor =  self.request(query)
        return [movie['name'] for movie in movies]

    def get_female_singers(self):
        '''
        '''
        query = [{
            'limit': 1000,
            'name': None,
            'type': '/music/artist',
            '/people/person/gender': 'Female',
            '/people/person/profession|=': [ 'Singer' ],
        }]
        for artists in self.paged_request(query):
            for artist in artists:
                yield artist['name']

if __name__ == "__main__":
    database = Freebase()
    for movie in database.get_movie_titles():
        print movie.encode('utf-8')
