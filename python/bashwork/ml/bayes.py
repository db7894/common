from collections import Counter

class Bayes(object):

    def train(self, dataset, labels):
        self.prior = Counter(labels)
        self.likes = Counter((l,f,v) for l, ds in zip(labels, dataset) for f, v in enumerate(ds))

    def classify(self, entry):
        pc = float(sum(self.prior.values()))
        fs = enumerate(entry)
        posterier = ((p / pc * sum(self.likes[(c,f,v)] for f,v in fs) / float(p), c) for c, p in self.prior.items())
        return max(posterier)[1]

if __name__ == "__main__":
    data = [
        [0, 0],
        [0, 1],
        [1, 0],
        [1, 1],
        [0, 0],
        [0, 1],
        [1, 0],
        [1, 1],
        [0, 0],
        [0, 1],
        [1, 0],
        [1, 1],
    ]
    lables = [0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0]
    bayes = Bayes()
    bayes.train(data, lables)
    print [(bayes.classify(d), l) for d,l in zip(data, lables)]
