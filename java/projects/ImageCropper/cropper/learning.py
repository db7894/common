import sys
from sklearn.svm import LinearSVC
from sklearn.ensemble import RandomForestClassifier
from sklearn.cross_validation import train_test_split
from sklearn.cross_validation import cross_val_score
from sklearn.grid_search import GridSearchCV


def train_classifier(model):
    ''' Given a model, train a classifier and print a
    few evaluation parameters to stdout.

    :param model: The classifier model to train
    :returns: The trained model
    '''
    model.fit(X_train, y_train)             # train the classifier with our test dataset
    print model.predict(X_train[:10])       # check our training set
    print y_train[:10]                      # see how it compares to real labels
    print model.score(X_train, y_train)     # check for bias in the training
    print model.score(X_test, y_test)       # perform our final evaluation
    return model

def train_random_tree():
    ''' This trains a random tree classifier and runs
    cross validation on the resulting model.
    '''
    system = RandomForestClassifier(n_estimators=50)
    forest = train_classifier(system)
    scores = cross_val_score(forest, X_train, y_train, cv=5)
    print("scores: %s  mean: %f  std: %f" % (str(scores), np.mean(scores), np.std(scores)))

def train_svm_classifier():
    ''' This trains an SVM classifier by performing a 
    brute force search on the margin parameter C.
    '''
    svm         = train_classifier(LinearSVC(C=0.1))
    param_grid  = {'C': 10. ** np.arange(-3, 4)}
    grid_search = GridSearchCV(svm, param_grid=param_grid, cv=3, verbose=3)
    grid_search.fit(X_train, y_train)
    print(grid_search.best_params_)
    print(grid_search.best_score_)

def load_database(database):
    ''' Given a path to the images database,
    return the converted dataset that can be
    injested by sklearn.

    :param database: The source of the image dataset
    :returns: (features, class of image 1/0)
    '''
    result = []
    for index in images['Path']:
        result.append((images['Features'][index], images['Class'][index]))
    return zip(*result)

if __name__ == "__main__":
    if len(sys.argv) <= 1:
        print "%s <database.json>" % sys.argv[0]
        sys.exit()

    database = json.load(open(sys.argv[1], 'r'))
    values, targets = load_database(database)
    datasplit = train_test_split(values, targets)
    X_train, X_test, y_train, y_test = datasplit
    train_svm_classifier()
