# --------------------------------------------------------------------------------
# import mnist data
# --------------------------------------------------------------------------------
import tensorflow as tf
from tensorflow.examples.tutorials.mnist import input_data

# --------------------------------------------------------------------------------
# configure settings
# --------------------------------------------------------------------------------

flags = tf.app.flags
FLAGS = flags.FLAGS
flags.DEFINE_string('data_dir', '/tmp/data/', 'Directory for storing data')

mnist = input_data.read_data_sets(FLAGS.data_dir, one_hot=True)
sess = tf.InteractiveSession()

# --------------------------------------------------------------------------------
# create the model
# --------------------------------------------------------------------------------
# y = Wx + b
# --------------------------------------------------------------------------------

x = tf.placeholder(tf.float32, [None, 784])
W = tf.Variable(tf.zeros([784, 10]))
b = tf.Variable(tf.zeros([10]))
y = tf.nn.softmax(tf.matmul(x, W) + b)

# --------------------------------------------------------------------------------
# define loss and optimizer
# --------------------------------------------------------------------------------
# H_y'(y) = -\sum_i y'_i * log(y_i)
# --------------------------------------------------------------------------------

y_ = tf.placeholder(tf.float32, [None, 10])
cross_entropy = tf.reduce_mean(-tf.reduce_sum(y_ * tf.log(y), reduction_indices=[1]))
train_step = tf.train.GradientDescentOptimizer(0.5).minimize(cross_entropy)

# --------------------------------------------------------------------------------
# train the model
# --------------------------------------------------------------------------------

tf.initialize_all_variables().run()
for i in range(1000):
    batch_xs, batch_ys = mnist.train.next_batch(100)
    train_step.run({x: batch_xs, y_: batch_ys})

# --------------------------------------------------------------------------------
# test the trained model
# --------------------------------------------------------------------------------
    correct_prediction = tf.equal(tf.argmax(y, 1), tf.argmax(y_, 1))
    accuracy = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))
    print(accuracy.eval({ x: mnist.test.images, y_: mnist.test.labels }))
