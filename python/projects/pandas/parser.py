import pandas as pd

# ------------------------------------------------------------
# Constants
# ------------------------------------------------------------

COLUMNS = [
    'age',
    'workclass',
    'weight',
    'education',
    'education-num',
    'marital-status',
    'occupation',
    'relationship',
    'race',
    'sex',
    'capital-gain',
    'capital-loss',
    'hours-per-week',
    'native-country',
    'label'
]

PATHS = {
    'training' : 'http://archive.ics.uci.edu/ml/machine-learning-databases/adult/adult.data',
    'testing'  : 'http://archive.ics.uci.edu/ml/machine-learning-databases/adult/adult.test',
}

# ------------------------------------------------------------
# Helper Utilities 
# ------------------------------------------------------------

def read_dataset(path, **kwargs):
    ''' Given a path to the dataset, return a cleaned and
    processed pandas data frame.

    :param path: The path (file or url) to the input data
    :returns: The cleaned pandas data frame
    '''
    params = {
        'names'            : COLUMNS,
        'sep'              : ',',
        'skipinitialspace' : True,
        'na_values'        : ['?'],
        'skiprows'         : 0,
        'skipfooter'       : 1
    }
    params.update(kwargs)
    frame = pd.read_csv(path, **params)
    frame = frame.drop('weight', axis=1)
    frame['label'] = frame['label'].str.contains('>50K').astype(int)
    #frame['occupation'] = frame['occupation'].value_counts()[0]
    return frame

# ------------------------------------------------------------
# Main 
# ------------------------------------------------------------
if __name__ == "__main__":

    frame = read_dataset('adult.test', skiprows=1)  
    print frame.head()
    print frame.describe()
