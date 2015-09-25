#!/usr/bin/env python
import unittest
from bashwork.structure.trie import Trie

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class TrieTest(unittest.TestCase):
    ''' Code to validate that the trie implementation is correct.
    '''

    def test_leaf_markers(self):
        ''' Test that the trie leaf markers work correctly '''
        trie = Trie()
        self.assertFalse(trie.has_a_word())
        trie.mark_word(True)
        self.assertTrue(trie.has_a_word())
        trie.unmark_word()
        self.assertFalse(trie.has_a_word())

    def test_create(self):
        ''' Test that the trie is created correctly '''
        words = ['hello', 'world']
        trie = Trie.create(words)
        for word in words:
            self.assertTrue(word in trie)
            self.assertTrue(trie.has_path(word))
            self.assertTrue(trie.get_path(word).root)

    def test_remove_word(self):
        ''' Test that a word can be removed from the trie correctly '''
        word = 'hello'
        trie = Trie.create([word])
        self.assertTrue(word in trie)
        trie.remove_word(word)
        self.assertFalse(word in trie)
        self.assertFalse(trie.has_path(word))
        self.assertFalse(trie.get_path(word).root)

    def test_clean_trie(self):
        ''' Test that the trie is cleaned correctly '''
        word = 'hello'
        trie = Trie.create([word])
        self.assertTrue(word in trie)
        path = trie.get_path(word)
        path.unmark_word()
        trie.clean()
        self.assertFalse(word in trie)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
