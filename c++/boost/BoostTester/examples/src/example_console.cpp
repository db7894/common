/**
 * @file example-console.cpp
 * @brief An example useage of the ConsoleHandler
 */
#include <iostream>
/* boost candy */
#include <boost/thread.hpp>
#include <boost/lexical_cast.hpp>
/* our candy */
#include "ConsoleHandler.h"

/**
 * @brief Example console runner
 */
class ExampleConsole : public ConsoleHandler
{
public:
	/**
	 * @brief RAII Constructor
	 */
	ExampleConsole(void) {
		if (connect_handlers()) {
			start();
		}
	};

	~ExampleConsole(void) {}

protected:
	/**
	 * @brief Callback from base console on stop
	 * @return void
	 */
	void stop_callback(void) {
		std::cout << "Stopping Main Running Thread\n";
	};

private:
	/**
	 * @brief Abstracted connector function
	 * @return true if we should start, false otherwise
	 */
	bool connect_handlers(void)
	{
		connect('A', boost::bind(&ExampleConsole::do_add, this),
			"Add Two Values");
		connect('S', boost::bind(&ExampleConsole::do_sub, this),
			"Subtract Two Values");
		connect('D', boost::bind(&ExampleConsole::do_div, this),
			"Divide Two Values");
		connect('M', boost::bind(&ExampleConsole::do_mul, this),
			"Multiply Two Values");
		return true;
	}

	typedef std::pair<double, double> value_pair;
	static value_pair get_values(void)
	{
		std::string left, right;
		std::cout << "Enter The First Value: ";
		std::cin  >> left;
		std::cout << "Enter The Second Value: ";
		std::cin  >> right;

		return std::make_pair(boost::lexical_cast<double>(left),
			boost::lexical_cast<double>(right));
	}

	void do_add(void) {
		value_pair o = get_values();
		std::cout << "Result: " << o.first + o.second << "\n";
	};
	void do_sub(void) {
		value_pair o = get_values();
		std::cout << "Result: " << o.first - o.second << "\n";
	};
	void do_div(void) {
		value_pair o = get_values();
		std::cout << "Result: " << o.first / o.second << "\n";
	};
	void do_mul(void) {
		value_pair o = get_values();
		std::cout << "Result: " << o.first * o.second << "\n";
	};
};

/**
 * @brief Example runner
 */
int main(void)
{
	ExampleConsole console;

	/* let console start up */
	while (!console.running());
	do {
		/* perform buisness logic here */
		boost::thread::yield();
	} while(console.running());

	return 0;
}
