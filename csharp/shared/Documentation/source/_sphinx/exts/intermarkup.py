from docutils import nodes, utils
from docutils.parsers.rst import roles
from sphinx import addnodes

#--------------------------------------------------------------------------------#
#   'keyword'   : ('Display Name', 'Format Link %s')
#--------------------------------------------------------------------------------#
_markups = {
    'bitbucket' : ('BitBucket', 'http://bitbucket.org/repo/all/?name=%s'),
    'gcode'     : ('Google Code', 'http://code.google.com/p/%s/'),
    'github'    : ('GitHub', 'http://github.com/%s/'),
    'google'    : ('Google', 'http://www.google.com/search?q=%s'),
    'launchpad' : ('Launchpad', 'https://launchpad.net/%s'),
    'pep'       : ('PEP', 'http://www.python.org/dev/peps/pep-%04d/'),
    'pydoc'     : ('Python Documentation', 'http://docs.python.org/search.html?q=%s'),
    'pypi'      : ('Python Package Index', 'http://pypi.python.org/pypi?%3Aaction=search&term=%s'),
    'recipe'    : ('ActiveState Code', 'http://code.activestate.com/recipes/%s/'),
    'rfc'       : ('RFC', 'http://www.ietf.org/rfc/rfc%s.txt'),
    'wiki'      : ('Wikipedia', 'http://www.wikipedia.com/wiki/%s'),
    'so'        : ('StackOverflow', 'http://stackoverflow.com/questions/%s/'),
    'sf'        : ('ServerFault', 'http://serverfault.com/questions/%s/'),
    'su'        : ('SuperUser', 'http://superuser.com/questions/%s/'),
    'msdn'      : ('MSDN', 'http://msdn.microsoft.com/en-us/library/%s.aspx'),
    'jonskeet'  : ('Jon Skeet', 'http://askjonskeet.com/search/?q=%s'),
    
}

def intermarkup(typ, rawtext, etext, lineno, inliner, options={}, content=[]):
    ''' Applies the shortcut to the lookup
    '''
    env = inliner.document.settings.env
    if not typ:
        typ = env.config.default_role
    else: typ = typ.lower()
    text = utils.unescape(etext)
    targetid = 'index-%s' % env.index_num
    env.index_num += 1
    indexnode  = addnodes.index()
    targetnode = nodes.target('', '', ids=[targetid])
    inliner.document.note_explicit_target(targetnode)

    if typ in _markups.keys():
        name, link = _markups[typ]
        indexnode['entries'] = [('single', _(name), targetid, name)]

        ref = link % text
        sn  = nodes.strong(text, text)
        rn  = nodes.reference('', '', refuri=ref)
        rn += sn
        return [indexnode, targetnode, rn], []

#--------------------------------------------------------------------------------#
# Add our shortcuts
#--------------------------------------------------------------------------------#
for key in _markups.keys():
    roles.register_local_role(key, intermarkup)
