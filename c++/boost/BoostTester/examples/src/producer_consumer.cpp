/**
 * @file producer_consumer.cpp
 * @brief A quick example of a consumer producer
 */
#include <sstream>
#include <boost/shared_ptr.hpp>
#include <boost/thread.hpp>
#include <boost/thread/mutex.hpp>
#include "Producer.hpp"
#include "Consumer.hpp"

/* our dummy message */
struct my_message { std::string message; };
typedef boost::shared_ptr<my_message> my_message_ptr;

/* our dummy producer */
void produce_thread(Producer<my_message> &producer)
{
	std::ostringstream stream;
	for (int i = 0; i < 1000; ++i) {
		my_message_ptr input(new my_message());
		stream << "Message: " << i;
		input->message = stream.str();
		producer.Produce(input);
		stream.str("");
	}
}

/* so our output isn't jumbled */
static boost::mutex print_mutex;

/* our dummy consumer */
class my_consumer : public Consumer<my_message>
{
public:
	my_consumer(IProducer<my_message>* producer, int id)
		: Consumer(producer), id_(id)
	{};

protected:
	bool callback(my_message_ptr input)
	{
 		boost::mutex::scoped_lock lock(print_mutex);
 		std::cout << "[" << id_ << "] " << input->message << "\n";
			return true;
	};

private:
	int id_;
};

/* our main runner */
int main(void)
{
	boost::thread_group manager;
	Producer<my_message> producer;
	my_consumer consumer1(&producer, 1);
	my_consumer consumer2(&producer, 2);

	/* create producer thread and wait for it to finish */
	manager.create_thread(boost::bind(&produce_thread,
		boost::ref(producer)));
	manager.join_all();
	
	/* wait for the messages to finish */
	boost::xtime t;
	while (producer.Size() != 0) {
		boost::xtime_get(&t, boost::TIME_UTC);
		t.sec += 1;
		boost::thread::sleep(t);
	}
	return 0;
}

