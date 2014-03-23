from baswwork.structure.tree.binary import BinaryNode
from bashwork.ml.common import Entry, count_groups, best_gain

class DecisionTree(BinaryNode):
    ''' Represents a simple decision tree. Each node in this tree
    should have the following attributes:

    * name  - a graphical name for this field
    * label - the label of this node (if this is a leaf)
    * field - the field to split on (if this is a tree node)
    * value - the value of the field to split with (if this is a tree node)
    '''

    def classify(self, entry):
        ''' Classify a new entry with the current tree
        
        :param entry: The entry to classify
        :returns: The label of the new entry
        '''
        node = self
        while not node.is_leaf_node():
            go_right = entry.values[node['field']] > node['value']
            node = node.right if go_right else node.left
        return node['label']

    def __str__(self):
        if self['label']: return self.label
        return "{}[{}] => {}".format(self['name'], self['field'], self['value'])

    @classmethod
    def train(klass, dataset, attributes, threshold=0.01):
        ''' Given a dataset, train a new decision tree
        with the supplied parameters.
        
        :param dataset: The training dataset
        :param attributes: The attribute names
        :param threshold: The gain threshold to cut off at
        :returns: A newly trained decision tree
        '''
        def tree_builder(data):
            if not len(data): return None                      # break if we have no data

            label, groups = count_groups(data)                 # if the groups are pure
            if (len(groups) == 1) or (len(attributes) == 0):   # or no more attributes
                return klass(label=label)                      # create a leaf label node

            (gain, value, field) = best_gain(data, attributes) # find the best split point
            if gain < threshold: return klass(label=label)     # unless the gain is too low

            node = klass(field=field, value=value, name=attributes[field])
            node.left  = tree_builder([e for e in data if e.values[field] <= value])
            node.right = tree_builder([e for e in data if e.values[field] >  value])
            return node
        return tree_builder(dataset)

#------------------------------------------------------------#
# helper methods
#------------------------------------------------------------#

def tree_to_rules(tree):
    ''' Given a decision tree, covert the tree
    to a collection of rules.
    
    :param tree: The tree to convert to rules
    :returns: A list of the rules encoded in the tree
    '''
    rules = []
    def rule_builder(node, rule):
        if not node.is_leaf_node():
            rule_builder(node.left,  rule + "%s <= %f, " % (node['name'], node['value']))
            rule_builder(node.right, rule + "%s >  %f, " % (node['name'], node['value']))
        else: rules.append(rule[0:-2] + " -> " + node['label'])
    rule_builder(tree)
    return rules

def tree_to_graphviz(tree):
    ''' Helper method to convert a tree to a graphviz
    document.
    
    :param tree: The tree to process
    :returns: A graphviz representation of the tree
    '''
    def get_tree_nodes(node, c):
        if not node.is_leaf_node():
            n = '\tnode%d [label ="%s"];\n' % (c, node['name'])
            (c, l) = get_tree_nodes(node.left,  c + 1)
            (c, r) = get_tree_nodes(node.right, c + 1)
            return (c, n + l + r)
        else: return (c, '\tnode%d [label ="%s"];\n' % (c, node['label']))

    def get_tree_links(node, count):
        if node.is_leaf_node(): return (count, "")
        n  = '\tnode%d -> node%d [label="<= %0.2f"]\n' % (count, count + 1, node.value)
        (c, l) = get_tree_links(node.left,  count+1)
        n += '\tnode%d -> node%d [label="> %0.2f"]\n'  % (count, c+1, node.value)
        (c, r) = get_tree_links(node.right, c+1)
        return (c, n + l + r)

    graph  = "digraph G\n{\n\tnode  [shape = diamond];\n"
    graph += '\tedge  [color="#2554c7"];\n'
    graph += get_tree_nodes(tree, 0)[1]
    graph += "\n\n"
    graph += get_tree_links(tree, 0)[1]
    return graph + "}"
