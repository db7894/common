'''
tensorboard --logdir=log_simple_graph
http://localhost:6006/#graphs
'''
import tensorflow as tf

x = tf.constant(1.0, name='input')
w = tf.Variable(0.8, name='weight')
y = tf.mul(x, w, name='output')
y_ = tf.constant(0.0, name='correct_value')
loss = tf.pow(y - y_, 2, 'loss')

train_step = optim = tf.train.GradientDescentOptimizer(
    learning_rate=0.025).minimize(loss)

for value in [x, w, y, y_, loss]:
    tf.scalar_summary(value.op.name, value)
summaries = tf.merge_all_summaries()

sess = tf.Session()
writer = tf.train.SummaryWriter('log_simple_graph', sess.graph)

sess.run(tf.initialize_all_variables())
for count in range(100):
    writer.add_summary(sess.run(summaries), count)
    sess.run(train_step)
