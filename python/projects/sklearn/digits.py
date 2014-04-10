from matplotlib.pylab import *
from sklearn.datasets import load_digits
from sklearn.svm import LinearSVC
from sklearn.ensemble import RandomForestClassifier
from sklearn.cross_validation import train_test_split
from sklearn.cross_validation import cross_val_score
from sklearn.decomposition import PCA
from sklearn.manifold import SpectralEmbedding
from sklearn.manifold import Isomap
from sklearn.grid_search import GridSearchCV

digits = load_digits()                                # load our dataset
splits = train_test_split(digits.data, digits.target) # randomly split our dataset
X_train, X_test, y_train, y_test = splits             # reserve for testing and training

def dimension_reduce():
    ''' This compares a few different methods of
    dimensionality reduction on the current dataset.
    '''
    pca = PCA(n_components=2)                             # initialize a dimensionality reducer
    pca.fit(digits.data)                                  # fit it to our data
    X_pca = pca.transform(digits.data)                    # apply our data to the transformation
    plt.subplot(1, 3, 1)
    plt.scatter(X_pca[:, 0], X_pca[:, 1], c=digits.target)# plot the manifold
    
    se = SpectralEmbedding()
    X_se = se.fit_transform(digits.data)
    plt.subplot(1, 3, 2)
    plt.scatter(X_se[:, 0], X_se[:, 1], c=digits.target)
    
    isomap = Isomap(n_components=2, n_neighbors=20)
    isomap.fit(digits.data)
    X_iso = isomap.transform(digits.data)
    plt.subplot(1, 3, 3)
    plt.scatter(X_iso[:, 0], X_iso[:, 1], c=digits.target)
    plt.show()

    plt.matshow(pca.mean_.reshape(8, 8))                  # plot the mean components
    plt.matshow(pca.components_[0].reshape(8, 8))         # plot the first principal component
    plt.matshow(pca.components_[1].reshape(8, 8))         # plot the second principal component
    plt.show()

def train_classifier(model):
    ''' Given a model, train a classifier and print a
    few evaluation parameters to stdout.

    :param model: The classifier model to train
    :returns: The trained model
    '''
    model.fit(X_train, y_train)                       # train the classifier with our test dataset
    print model.predict(X_train[:10])                 # check our training set
    print y_train[:10]                                # see how it compares to real labels
    print model.score(X_train, y_train)               # check for bias in the training
    print model.score(X_test, y_test)                 # perform our final evaluation
    return model

def train_random_tree():
    ''' This trains a random tree classifier and runs
    cross validation on the resulting model.
    '''
    rf = train_classifier(RandomForestClassifier(n_estimators=50))
    scores = cross_val_score(rf, X_train, y_train, cv=5)
    print("scores: %s  mean: %f  std: %f" % (str(scores), np.mean(scores), np.std(scores)))

def train_svm_classifier():
    ''' This trains an SVM classifier by performing a 
    brute force search on the margin parameter C.
    '''
    svm = train_classifier(LinearSVC(C=0.1))
    param_grid = {'C': 10. ** np.arange(-3, 4)}
    grid_search = GridSearchCV(svm, param_grid=param_grid, cv=3, verbose=3)
    grid_search.fit(X_train, y_train)
    print(grid_search.best_params_)
    print(grid_search.best_score_)

