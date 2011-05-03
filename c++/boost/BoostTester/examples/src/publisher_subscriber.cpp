/**
 * @file publisher_subscriber.cpp
 * @brief A simple publisher and subscriber
 */
#include <sstream>
#include <boost/shared_ptr.hpp>
#include <boost/thread.hpp>
#include <boost/thread/mutex.hpp>
#include "Publisher.hpp"
#include "Subscriber.hpp"

/* our dummy message */
struct my_message { std::string message; };
typedef boost::shared_ptr<my_message> my_message_ptr;

/* our dummy producer */
void produce_thread(Publisher<my_message> &publisher)
{
	std::ostringstream stream;
	for (int i = 0; i < 1000; ++i) {
		my_message_ptr input(new my_message());
		stream << "Message: " << i;
		input->message = stream.str();
		publisher.Publish(input);
		stream.str("");
	}
}

/* so our output isn't jumbled */
static boost::mutex print_mutex;

/* our dummy subscriber */
class my_subscriber : public Subscriber<my_message>
{
public:
	my_subscriber(int id)
		: id_(id)
	{};

protected:
	bool Work(my_message_ptr input) {
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
	Publisher<my_message> publisher;

	/* create a few subscribers */
	boost::thread_group manager;
	for (int i = 0; i < 3; ++i) {
		boost::shared_ptr<my_subscriber> handle(new my_subscriber(i));
		publisher.Subscribe(handle);
	}

	/* create producer thread and wait for it to finish */
	manager.create_thread(boost::bind(&produce_thread,
		boost::ref(publisher)));
	manager.join_all();

	/* wait for the messages to finish */
	boost::xtime t;
	while (publisher.Size() != 0) {
		boost::xtime_get(&t, boost::TIME_UTC);
		t.sec += 1;
		boost::thread::sleep(t);
	}
	return 0;
}

