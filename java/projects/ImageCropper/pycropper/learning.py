import sys
from sklearn.svm import LinearSVC
from sklearn.ensemble import RandomForestClassifier
from sklearn.cross_validation import train_test_split
from sklearn.cross_validation import cross_val_score
from sklearn.grid_search import GridSearchCV

class TrainContext(object):
    ''' A simple context for holding the training
    data for training a classifier model.
    '''

    __slots__ = ['x_train', 'y_train', 'x_test', 'y_test']

    @classmethod
    def create(klass, values, labels):
        ''' Given the values to train on and their
        associated labels, build a training context
        with data spilts for test and training.

        :param values: The values to train on
        :param labels: The labels for each value set
        :returns: An initialized TrainContext
        '''
        splits = train_test_split(values, labels)
        Xt, Yt, xt, yt = splits
        return klass(x_train=Xt, y_train=Yt, x_test=xt, y_test=yt)

    def __init__(self, **kwargs):
        ''' Initailizes a new instance of the TrainContext

        :param x_train: The values for the training set
        :param y_train: The labels for the training set
        :param x_test: The values for the testing set
        :param y_test: The labels for the testing set
        '''
        self.x_train = kwargs.get('x_train')
        self.y_train = kwargs.get('y_train')
        self.x_test  = kwargs.get('x_test')
        self.y_test  = kwargs.get('y_test')


def train_classifier(model, context):
    ''' Given a model, train a classifier and print a
    few evaluation parameters to stdout.

    :param model: The classifier model to train
    :param context: The training context to operate with
    :returns: The trained model
    '''
    C = context
    model.fit(C.x_train, C.y_train)         # train the classifier with our test dataset
    print model.predict(C.x_train[:10])     # check our training set
    print C.y_train[:10]                    # see how it compares to real labels
    print model.score(C.x_train, C.y_train) # check for bias in the training
    print model.score(C.x_test, C.y_test)   # perform our final evaluation
    return model


def train_random_tree_classifier(context):
    ''' This trains a random tree classifier and runs
    cross validation on the resulting model.

    :param context: The training context to operate with
    '''
    system = RandomForestClassifier(n_estimators=50)
    forest = train_classifier(system, context)
    scores = cross_val_score(forest, context.x_train, context.y_train, cv=5)
    print("scores: %s  mean: %f  std: %f" % (str(scores), np.mean(scores), np.std(scores)))


def train_svm_classifier(context):
    ''' This trains an SVM classifier by performing a 
    brute force search on the margin parameter C.

    :param context: The training context to operate with
    '''
    svm         = train_classifier(LinearSVC(C=0.1), context)
    param_grid  = {'C': 10. ** np.arange(-3, 4)}
    grid_search = GridSearchCV(svm, param_grid=param_grid, cv=3, verbose=3)
    grid_search.fit(context.x_train, context.y_train)
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
    values, labels = load_database(database)
    context = TrainContext.create(values, labels)
    train_svm_classifier(context)
