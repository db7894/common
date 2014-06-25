# Description:
#   A way to look up phone tool images
#
# Commands:
#   hubot phone <username> - Get the phone tool image for a user

module.exports = (robot) ->
  robot.respond /(phone)( me)? (.*)/i, (msg) ->
    imageMe msg, msg.match[3], (url) ->
      msg.send url

imageMe = (msg, query, callback) ->
  callback "http://badgephotos.amazon.com/?uid=" + query + "#.png"
