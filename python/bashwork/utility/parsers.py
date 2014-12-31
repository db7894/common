from xml.etree import ElementTree

def xml_to_dict(xml):
    ''' Given an XML string, convert it to a python dictionary
    of the form:

    * all attributes are dict entries of the current level
    * all children are dict attributes of the current level
    * repeated elements are a list attribute of the current level

    :param xml: The XML to convert to a dictionary
    :returns: The converted dictionary
    '''
    def parse(element):
        is_dict = (len(element) == 1) or (element[0].tag != element[1].tag)
        parser  = parse_xml_dict if is_dict else parse_xml_list
        return parser(element)

    def parse_xml_list(parent):
        values = []
        result = dict(parent.items())
        result[parent[0].tag + 's'] = values
        for element in parent:
            is_root = len(element) == 0
            values.append(element.text if is_root else parse(element))
        return values

    def parse_xml_dict(parent):
        values = dict(parent.items())
        for element in parent:
            is_root = len(element) == 0
            values[element.tag] = element.text if is_root else parse(element)
        return values

    return parse(ElementTree.fromstring(xml))

def parse_multipart_binary(self, binary, boundary='--myboundary'):
    ''' Given a binary payload and a supplied boundary, parse
    the binary into N binary files along with their associated
    metadata (between boundaries).

    :param binary: The binary to split
    :param boundary: The boundary seperating the files
    :returns [ ({ metadata }, binary) ]
    '''
    index = binary.find(boundary)                    # start at the first boundary if it exists
    parts = []                                       # the collection of parts in this blob 
    while index != -1:                               # until there are no more parts in this blob
        keys = {}                                    # the keys that are stored with this multipart
        part = binary.find('\n', index)              # find the end of the boundary
        endl = binary.find('\n', part + 1)           # find the end of the k:v pair
        while (part + 3) < endl:                     # while we are not on an empty newline (CR or CRLF)
            entry = binary[part + 1:endl]            # extract the k:v pair in range
            key, value = entry.split(':')            # split the k:v pair
            keys[key.strip()] = value.strip()        # clean and store the pair
            part = endl                              # set start at end of current k:v pair
            endl = binary.find('\n', part + 1)       # move to end of next k:v pair
        new_index = binary.find(boundary, index + 1) # get the next boundary
        part = binary.find('\n', index) + 1          # skip past the boundary
        parts.append((keys, binary[part:new_index])) # add the new blob
        index = new_index                            # prepare for next multipart
    return parts or [({}, binary)]                   # the parts or no multipart ever existed
