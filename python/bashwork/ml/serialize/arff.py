'''
.. todo::
   - load and dump
   - get model headers as well into entry
   - http://www.cs.waikato.ac.nz/ml/weka/arff.html
'''
from bashwork.ml.common import Entry
from bashwork.generators import gen_file_stream

#------------------------------------------------------------#
# arff helpers
#------------------------------------------------------------#

def __parse_entry_line(entry):
	''' Parses a single line of an arff file.

    :param entry: The arff entry to parse
    :returns: The parsed arff entry
	'''
	pieces = entry.split(",")
	label  = pieces.pop().strip()
	values = tuple([float(piece) for piece in pieces])
	return Entry(label, values)

#------------------------------------------------------------#
# exports
#------------------------------------------------------------#
		
def load(path):
	''' Load an ARFF file and return the entries inside.

    :param path: The file path to load
    :returns: A generator of the arff entries
	'''
	for line in gen_file_stream(path):
		if not line.startswith("@"):
			yield __parse_entry_line(line)
