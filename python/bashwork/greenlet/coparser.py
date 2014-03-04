import re

def is_open_tag(token):
    return token.startswith("<") and not token.startswith("</")

def co_parse_items(close_tag=None):
    elements = []
    while True:
        element = yield
        if not element: break # EOF
        if is_open_tag(element):
            parsed = yield from co_parse_elements(element)
            elements.append((element, parsed))
        elif element == close_tag: break
        else: elements.append(element)
    return elements

def co_parse_elements(open_tag):
    name = open_tag[1:-1]
    close_tag = "</%s>" % name
    items = yield from co_parse_items(close_tag)
    return items

def co_parse(text, pattern=r"(\S+)|(<[^>]*>)"):
  pattern = re.compile(pattern)
  parser  = co_parse_items()
  parser.send(None)
  try:
    for match in pattern.finditer(text):
      token = match.group(0)
      #print("Feeding: %s", token)
      parser.send(token)
    parser.send(None) # to signal EOF
  except StopIteration as e: return e.args[0]

text = "<foo> This is a <b> foo file </b> you know. </foo>"
print(co_parse(text))
