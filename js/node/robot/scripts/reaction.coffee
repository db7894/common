# Description:
#   A way to interact with the Google Images API.
#
# Commands:
#   hubot react to <query> - Query reaction gifs for a reaction gif
cheerio = require 'cheerio'

module.exports = (robot) ->
  robot.respond /(react)( to)? (.*)/i, (msg) ->
    imageMe msg, msg.match[3], (url) ->
      msg.send url

imageMe = (msg, query, callback) ->
  q = s: query, submit: 'Search'
  msg.http('http://www.reactiongifs.com')
    .query(q)
    .get() (err, res, body) ->
      $ = cheerio.load(body)
      images = $('.entry img')
      if images?.length > 0
        image  = msg.random images
        callback image.attribs.src

