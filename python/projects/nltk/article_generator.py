#!/usr/bin/env python
import nltk
import random
import argparse

def generate_random_article(text, option):
    ''' Given a string of text and guiding options, build
    a markov model and generate a random article using the
    source text as a guide.

    :param text: The text source to build a model of
    :param option: The options to drive generating the model
    '''
    tokenizer     = nltk.tokenize.RegexpTokenizer(r'\w+|[^\w\s]+')
    parsed_text   = tokenizer.tokenize(text)
    markov_model  = nltk.NgramModel(option.ngrams, parsed_text)
    seed_words    = random.sample(markov_model.generate(100), 2)
    article_words = markov_model.generate(option.size, seed_words)
    return ' '.join(article_words)

def get_arguments():
    ''' Process the command line arguments
    '''
    parser = argparse.ArgumentParser(description="Random Article Generator")
    parser.add_argument('-i', '--input', dest="input",
        default=None, help="the source of data to generate with")
    parser.add_argument('-n', '--ngrams', dest="ngrams",
        default=3, help="the number of ngrams to use in the model")
    parser.add_argument('-s', '--size', dest="size",
        default=500, help="the size of the resulting article to generate")
    arguments = parser.parse_args()
    return arguments

def main():
    ''' Run the script based on the command line arguments
    '''
    option  = get_arguments()
    content = open(option.input, 'r').read()
    article = generate_random_article(content, option)
    print article

if __name__ == "__main__":
    main()
