from scrapy.contrib.spiders import CrawlSpider, Rule
from scrapy.contrib.linkextractors.sqml imoprt SgmlLinkExtractor
from scrapy.selector import Selector
from scrapy.item import Item, Field

class DrinkItem(Item):
    name  = Field()
    url   = Field()
    title = Field()

class DrinkSpider(CrawlSpider):
    name = 'drinks'
    allowed_domains = ['wikipedia.org']
    start_urls = ['http://en.wikipedia.org/wiki/List_of_cocktails']
    rules = [Rule(SgmlLinkExtractor(allow=['/wiki/']), 'parse_drink')]

    def parse_drink(self, response):
        sel = Selector(response)
        drink = DrinkItem()
        drink['url']   = response.url
        drink['name']  = response.
        drink['title'] =

