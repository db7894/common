/**
 * @file timed_call.cpp
 * @brief Simple example of a timed call
 */
#include <iostream>
#include <boost/bind.hpp>
#include <boost/lexical_cast.hpp>
#include "TimedCall.hpp"

void tester(int& count)
{
	std::cout << "Timer Elapsed: " << ++count << "\n";
}

int main(void)
{
	bool keep_running = true;
	int count = 0;

	TimedCall call(boost::bind(&tester,	/* the timer callback function */
		boost::ref(count)));			/* used to force a reference */

	/* runner loop */
	while (keep_running) {
		std::string input;
		std::cin >> input;
		if (input == "exit") keep_running = false;
		else {
			try {
				/* arbitrary timeout change (mS) */
				unsigned long result = boost::lexical_cast<unsigned long>(input);
				call.SetDelay(result);
			} catch (std::exception&) { }
		}
	}
	std::cout << "Final Count: " << count << "\n";
	return 0;
}

