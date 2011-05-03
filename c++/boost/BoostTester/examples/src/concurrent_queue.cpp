/**
 * @file concurrent_queue.cpp
 * @brief Example of using the concurrent queue
 */
#include <sstream>
#include <boost/shared_ptr.hpp>
#include <boost/thread.hpp>
#include "ConcurrentQueue.hpp"

struct my_message { std::string message; };
typedef boost::shared_ptr<my_message> my_message_t;

void produce_thread(ConcurrentQueue<my_message_t> &queue)
{
  std::ostringstream stream;
  for (int i = 0; i < 1000; ++i) {
      my_message_t input(new my_message());
      stream << "Message: " << i;
      input->message = stream.str();
      queue.Produce(input);
      stream.str("");
  }
}

void consume_thread(ConcurrentQueue<my_message_t> &queue)
{
  for (int i = 0; i < 1000; ++i) {
      my_message_t input = queue.Consume();
      std::cout << input->message << "\n";
  }
}

int main(void)
{
  ConcurrentQueue<my_message_t> queue_;
  boost::thread_group manager;

  manager.create_thread(boost::bind(&consume_thread, boost::ref(queue_)));
  manager.create_thread(boost::bind(&produce_thread, boost::ref(queue_)));
  manager.join_all();

  return 0;
}

